/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using BlackSharp.Core.Reflection.Emit;
using System.Security.Cryptography;

namespace BlackSharp.Core.Security.Cryptography
{
    public delegate object SHA3NewDelegate();

    public abstract class SHA3 : HashAlgorithm
    {
        #region Constructor

        static SHA3()
        {
            var dict = new Dictionary<string, SHA3NewDelegate>()
            {
                { nameof(SHA3_224), () => new SHA3_224() },
                { nameof(SHA3_256), () => new SHA3_256() },
                { nameof(SHA3_384), () => new SHA3_384() },
                { nameof(SHA3_512), () => new SHA3_512() },
            };

            RegisteredClasses = dict;
        }

        protected SHA3(int hashBitSize)
        {
            Initialize();
            KeccakR = 200 - (2 * hashBitSize / 8);
            HashSizeValue = hashBitSize;
        }

        #endregion

        #region Constants

        public const int KeccakB              = 1600 ;
        public const int KeccakNumberOfRounds = 24   ;
        public const int KeccakLaneSizeInBits = 8 * 8;

        protected static readonly ulong[] RoundConstants = new ulong[]
        {
            0x0000000000000001UL,
            0x0000000000008082UL,
            0x800000000000808aUL,
            0x8000000080008000UL,
            0x000000000000808bUL,
            0x0000000080000001UL,
            0x8000000080008081UL,
            0x8000000000008009UL,
            0x000000000000008aUL,
            0x0000000000000088UL,
            0x0000000080008009UL,
            0x000000008000000aUL,
            0x000000008000808bUL,
            0x800000000000008bUL,
            0x8000000000008089UL,
            0x8000000000008003UL,
            0x8000000000008002UL,
            0x8000000000000080UL,
            0x000000000000800aUL,
            0x800000008000000aUL,
            0x8000000080008081UL,
            0x8000000000008080UL,
            0x0000000080000001UL,
            0x8000000080008008UL
        };

        #endregion
        
        #region Fields

        static readonly IReadOnlyDictionary<string, SHA3NewDelegate> RegisteredClasses;

        protected ulong[] _State     ;
        protected byte[]  _Buffer    ;
        protected int     _BuffLength;

        #endregion

        #region Properties

        public override bool CanReuseTransform { get { return true; } }

        /// <summary>
        /// AKA Size in bytes.
        /// </summary>
        protected int KeccakR { get; private set; }
        protected int HashByteLength { get { return HashSizeValue / 8; } }

        #endregion

        #region Public

        public static new SHA3 Create()
        {
            return Create(nameof(SHA3_512));
        }

        public static new SHA3 Create(string hashName)
        {
            SHA3NewDelegate item;
            if (RegisteredClasses.TryGetValue(hashName, out item))
            {
                return item() as SHA3;
            }
            else
            {
                return null;
            }
        }

        public override void Initialize()
        {
            _BuffLength = 0;
            _State      = new ulong[5 * 5]; //1600 bits
        }

        #endregion

        #region Protected

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (ibStart < 0)
                throw new ArgumentOutOfRangeException(nameof(ibStart));
            if (cbSize > array.Length)
                throw new ArgumentOutOfRangeException(nameof(cbSize));
            if (ibStart + cbSize > array.Length)
                throw new ArgumentOutOfRangeException($"{nameof(ibStart)} or {nameof(cbSize)}");

            if (cbSize == 0)
            {
                return;
            }

            int sizeInBytes = KeccakR;

            if (_Buffer == null)
            {
                _Buffer = new byte[sizeInBytes];
            }

            int stride = sizeInBytes >> 3;
            ulong[] utemps = new ulong[stride];

            if (_BuffLength == sizeInBytes)
            {
                throw new Exception("The internal buffer is full.");
            }

            AddToBuffer(array, ref ibStart, ref cbSize);

            if (_BuffLength == sizeInBytes) //Buffer full
            {
                Buffer.BlockCopy(_Buffer, 0, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
                _BuffLength = 0;
            }

            for (; cbSize >= sizeInBytes; cbSize -= sizeInBytes, ibStart += sizeInBytes)
            {
                Buffer.BlockCopy(array, ibStart, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
            }

            if (cbSize > 0) //Some left over
            {
                Buffer.BlockCopy(array, ibStart, _Buffer, _BuffLength, cbSize);
                _BuffLength += cbSize;
            }
        }

        protected override byte[] HashFinal()
        {
            int sizeInBytes = KeccakR;
            byte[] outb = new byte[HashByteLength];

            //Padding
            if (_Buffer == null)
            {
                _Buffer = new byte[sizeInBytes];
            }
            else
            {
                Array.Clear(_Buffer, _BuffLength, sizeInBytes - _BuffLength);
            }

            _Buffer[_BuffLength++] ^= 0x06; //SHA3 -> For Keccak use 0x01 instead
            _Buffer[sizeInBytes - 1] ^= 0x80;

            int stride = sizeInBytes >> 3;
            ulong[] utemps = new ulong[stride];

            Buffer.BlockCopy(_Buffer, 0, utemps, 0, sizeInBytes);

            KeccakF(utemps, stride);

            Buffer.BlockCopy(_State, 0, outb, 0, HashByteLength);

            return outb;
        }

        protected ulong ROL(ulong a, int offset)
        {
            return (((a) << ((offset) % KeccakLaneSizeInBits)) ^ ((a) >> (KeccakLaneSizeInBits - ((offset) % KeccakLaneSizeInBits))));
        }

        protected void AddToBuffer(byte[] array, ref int offset, ref int count)
        {
            int amount = Math.Min(count, _Buffer.Length - _BuffLength);

            Buffer.BlockCopy(array, offset, _Buffer, _BuffLength, amount);

            offset += amount;
            _BuffLength += amount;
            count -= amount;
        }

        #endregion

        #region Private API

        void KeccakF(ulong[] inb, int laneCount)
        {
            while (--laneCount >= 0)
                _State[laneCount] ^= inb[laneCount];
            ulong Aba, Abe, Abi, Abo, Abu;
            ulong Aga, Age, Agi, Ago, Agu;
            ulong Aka, Ake, Aki, Ako, Aku;
            ulong Ama, Ame, Ami, Amo, Amu;
            ulong Asa, Ase, Asi, Aso, Asu;
            ulong BCa, BCe, BCi, BCo, BCu;
            ulong Da, De, Di, Do, Du;
            ulong Eba, Ebe, Ebi, Ebo, Ebu;
            ulong Ega, Ege, Egi, Ego, Egu;
            ulong Eka, Eke, Eki, Eko, Eku;
            ulong Ema, Eme, Emi, Emo, Emu;
            ulong Esa, Ese, Esi, Eso, Esu;
            int round = laneCount;

            //copyFromState(A, state)
            Aba = _State[0];
            Abe = _State[1];
            Abi = _State[2];
            Abo = _State[3];
            Abu = _State[4];
            Aga = _State[5];
            Age = _State[6];
            Agi = _State[7];
            Ago = _State[8];
            Agu = _State[9];
            Aka = _State[10];
            Ake = _State[11];
            Aki = _State[12];
            Ako = _State[13];
            Aku = _State[14];
            Ama = _State[15];
            Ame = _State[16];
            Ami = _State[17];
            Amo = _State[18];
            Amu = _State[19];
            Asa = _State[20];
            Ase = _State[21];
            Asi = _State[22];
            Aso = _State[23];
            Asu = _State[24];

            for (round = 0; round < KeccakNumberOfRounds; round += 2)
            {
                //prepareTheta
                BCa = Aba ^ Aga ^ Aka ^ Ama ^ Asa;
                BCe = Abe ^ Age ^ Ake ^ Ame ^ Ase;
                BCi = Abi ^ Agi ^ Aki ^ Ami ^ Asi;
                BCo = Abo ^ Ago ^ Ako ^ Amo ^ Aso;
                BCu = Abu ^ Agu ^ Aku ^ Amu ^ Asu;

                //thetaRhoPiChiIotaPrepareTheta(round, A, E)
                Da = BCu ^ ROL(BCe, 1);
                De = BCa ^ ROL(BCi, 1);
                Di = BCe ^ ROL(BCo, 1);
                Do = BCi ^ ROL(BCu, 1);
                Du = BCo ^ ROL(BCa, 1);

                Aba ^= Da;
                BCa = Aba;
                Age ^= De;
                BCe = ROL(Age, 44);
                Aki ^= Di;
                BCi = ROL(Aki, 43);
                Amo ^= Do;
                BCo = ROL(Amo, 21);
                Asu ^= Du;
                BCu = ROL(Asu, 14);
                Eba = BCa ^ ((~BCe) & BCi);
                Eba ^= RoundConstants[round];
                Ebe = BCe ^ ((~BCi) & BCo);
                Ebi = BCi ^ ((~BCo) & BCu);
                Ebo = BCo ^ ((~BCu) & BCa);
                Ebu = BCu ^ ((~BCa) & BCe);

                Abo ^= Do;
                BCa = ROL(Abo, 28);
                Agu ^= Du;
                BCe = ROL(Agu, 20);
                Aka ^= Da;
                BCi = ROL(Aka, 3);
                Ame ^= De;
                BCo = ROL(Ame, 45);
                Asi ^= Di;
                BCu = ROL(Asi, 61);
                Ega = BCa ^ ((~BCe) & BCi);
                Ege = BCe ^ ((~BCi) & BCo);
                Egi = BCi ^ ((~BCo) & BCu);
                Ego = BCo ^ ((~BCu) & BCa);
                Egu = BCu ^ ((~BCa) & BCe);

                Abe ^= De;
                BCa = ROL(Abe, 1);
                Agi ^= Di;
                BCe = ROL(Agi, 6);
                Ako ^= Do;
                BCi = ROL(Ako, 25);
                Amu ^= Du;
                BCo = ROL(Amu, 8);
                Asa ^= Da;
                BCu = ROL(Asa, 18);
                Eka = BCa ^ ((~BCe) & BCi);
                Eke = BCe ^ ((~BCi) & BCo);
                Eki = BCi ^ ((~BCo) & BCu);
                Eko = BCo ^ ((~BCu) & BCa);
                Eku = BCu ^ ((~BCa) & BCe);

                Abu ^= Du;
                BCa = ROL(Abu, 27);
                Aga ^= Da;
                BCe = ROL(Aga, 36);
                Ake ^= De;
                BCi = ROL(Ake, 10);
                Ami ^= Di;
                BCo = ROL(Ami, 15);
                Aso ^= Do;
                BCu = ROL(Aso, 56);
                Ema = BCa ^ ((~BCe) & BCi);
                Eme = BCe ^ ((~BCi) & BCo);
                Emi = BCi ^ ((~BCo) & BCu);
                Emo = BCo ^ ((~BCu) & BCa);
                Emu = BCu ^ ((~BCa) & BCe);

                Abi ^= Di;
                BCa = ROL(Abi, 62);
                Ago ^= Do;
                BCe = ROL(Ago, 55);
                Aku ^= Du;
                BCi = ROL(Aku, 39);
                Ama ^= Da;
                BCo = ROL(Ama, 41);
                Ase ^= De;
                BCu = ROL(Ase, 2);
                Esa = BCa ^ ((~BCe) & BCi);
                Ese = BCe ^ ((~BCi) & BCo);
                Esi = BCi ^ ((~BCo) & BCu);
                Eso = BCo ^ ((~BCu) & BCa);
                Esu = BCu ^ ((~BCa) & BCe);

                //    prepareTheta
                BCa = Eba ^ Ega ^ Eka ^ Ema ^ Esa;
                BCe = Ebe ^ Ege ^ Eke ^ Eme ^ Ese;
                BCi = Ebi ^ Egi ^ Eki ^ Emi ^ Esi;
                BCo = Ebo ^ Ego ^ Eko ^ Emo ^ Eso;
                BCu = Ebu ^ Egu ^ Eku ^ Emu ^ Esu;

                //thetaRhoPiChiIotaPrepareTheta(round + 1, E, A)
                Da = BCu ^ ROL(BCe, 1);
                De = BCa ^ ROL(BCi, 1);
                Di = BCe ^ ROL(BCo, 1);
                Do = BCi ^ ROL(BCu, 1);
                Du = BCo ^ ROL(BCa, 1);

                Eba ^= Da;
                BCa = Eba;
                Ege ^= De;
                BCe = ROL(Ege, 44);
                Eki ^= Di;
                BCi = ROL(Eki, 43);
                Emo ^= Do;
                BCo = ROL(Emo, 21);
                Esu ^= Du;
                BCu = ROL(Esu, 14);
                Aba = BCa ^ ((~BCe) & BCi);
                Aba ^= RoundConstants[round + 1];
                Abe = BCe ^ ((~BCi) & BCo);
                Abi = BCi ^ ((~BCo) & BCu);
                Abo = BCo ^ ((~BCu) & BCa);
                Abu = BCu ^ ((~BCa) & BCe);

                Ebo ^= Do;
                BCa = ROL(Ebo, 28);
                Egu ^= Du;
                BCe = ROL(Egu, 20);
                Eka ^= Da;
                BCi = ROL(Eka, 3);
                Eme ^= De;
                BCo = ROL(Eme, 45);
                Esi ^= Di;
                BCu = ROL(Esi, 61);
                Aga = BCa ^ ((~BCe) & BCi);
                Age = BCe ^ ((~BCi) & BCo);
                Agi = BCi ^ ((~BCo) & BCu);
                Ago = BCo ^ ((~BCu) & BCa);
                Agu = BCu ^ ((~BCa) & BCe);

                Ebe ^= De;
                BCa = ROL(Ebe, 1);
                Egi ^= Di;
                BCe = ROL(Egi, 6);
                Eko ^= Do;
                BCi = ROL(Eko, 25);
                Emu ^= Du;
                BCo = ROL(Emu, 8);
                Esa ^= Da;
                BCu = ROL(Esa, 18);
                Aka = BCa ^ ((~BCe) & BCi);
                Ake = BCe ^ ((~BCi) & BCo);
                Aki = BCi ^ ((~BCo) & BCu);
                Ako = BCo ^ ((~BCu) & BCa);
                Aku = BCu ^ ((~BCa) & BCe);

                Ebu ^= Du;
                BCa = ROL(Ebu, 27);
                Ega ^= Da;
                BCe = ROL(Ega, 36);
                Eke ^= De;
                BCi = ROL(Eke, 10);
                Emi ^= Di;
                BCo = ROL(Emi, 15);
                Eso ^= Do;
                BCu = ROL(Eso, 56);
                Ama = BCa ^ ((~BCe) & BCi);
                Ame = BCe ^ ((~BCi) & BCo);
                Ami = BCi ^ ((~BCo) & BCu);
                Amo = BCo ^ ((~BCu) & BCa);
                Amu = BCu ^ ((~BCa) & BCe);

                Ebi ^= Di;
                BCa = ROL(Ebi, 62);
                Ego ^= Do;
                BCe = ROL(Ego, 55);
                Eku ^= Du;
                BCi = ROL(Eku, 39);
                Ema ^= Da;
                BCo = ROL(Ema, 41);
                Ese ^= De;
                BCu = ROL(Ese, 2);
                Asa = BCa ^ ((~BCe) & BCi);
                Ase = BCe ^ ((~BCi) & BCo);
                Asi = BCi ^ ((~BCo) & BCu);
                Aso = BCo ^ ((~BCu) & BCa);
                Asu = BCu ^ ((~BCa) & BCe);
            }

            //copyToState(state, A)
            _State[0] = Aba;
            _State[1] = Abe;
            _State[2] = Abi;
            _State[3] = Abo;
            _State[4] = Abu;
            _State[5] = Aga;
            _State[6] = Age;
            _State[7] = Agi;
            _State[8] = Ago;
            _State[9] = Agu;
            _State[10] = Aka;
            _State[11] = Ake;
            _State[12] = Aki;
            _State[13] = Ako;
            _State[14] = Aku;
            _State[15] = Ama;
            _State[16] = Ame;
            _State[17] = Ami;
            _State[18] = Amo;
            _State[19] = Amu;
            _State[20] = Asa;
            _State[21] = Ase;
            _State[22] = Asi;
            _State[23] = Aso;
            _State[24] = Asu;
        }

        #endregion
    }
}
