/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 */

using BlackSharp.Core.Reflection.Emit;

namespace BlackSharp.Core.Tests.Reflection.Emit
{
    [TestClass]
    public class DynamicMethodDelegateFactoryTests
    {
        #region Delegates

        private delegate object TryParseIntDelegate(string s, out int result);
        private delegate object TryParseDoubleDelegate(string s, out double result);
        private delegate object TryParseFloatDelegate(string s, out float result);

        #endregion

        #region Public

        [TestMethod]
        public void CreateMethodAllocator_DefaultConstructor_ReturnsNewInstance()
        {
            var allocate = DynamicMethodDelegateFactory.CreateMethodAllocator<Func<object>>(typeof(SampleObject));

            var instance = allocate();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType<SampleObject>(instance);
        }

        [TestMethod]
        public void CreateMethodAllocator_ParameterizedConstructor_ReturnsNewInstanceWithValue()
        {
            const string expected = "hello";

            var allocate = DynamicMethodDelegateFactory.CreateMethodAllocator<Func<string, object>>(
                typeof(StringHolder),
                typeof(string));

            var instance = allocate(expected) as StringHolder;

            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        [TestMethod]
        public void CreateMethodAllocator_MultipleCallsReturnDistinctInstances()
        {
            var allocate = DynamicMethodDelegateFactory.CreateMethodAllocator<Func<object>>(typeof(SampleObject));

            var instance1 = allocate();
            var instance2 = allocate();

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void CreateMethodCaller_IntTryParse_ValidInput()
        {
            var method = typeof(int).GetMethod(nameof(int.TryParse), [typeof(string), typeof(int).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseIntDelegate>(method, typeof(object));

            var returned = (bool)caller("123", out int result);

            Assert.IsTrue(returned);
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void CreateMethodCaller_IntTryParse_InvalidInput()
        {
            var method = typeof(int).GetMethod(nameof(int.TryParse), [typeof(string), typeof(int).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseIntDelegate>(method, typeof(object));

            var returned = (bool)caller("not_a_number", out int result);

            Assert.IsFalse(returned);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CreateMethodCaller_DoubleTryParse_ValidInput()
        {
            var method = typeof(double).GetMethod(nameof(double.TryParse), [typeof(string), typeof(double).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseDoubleDelegate>(method, typeof(object));

            var returned = (bool)caller("100", out double result);

            Assert.IsTrue(returned);
            Assert.AreEqual(100d, result);
        }

        [TestMethod]
        public void CreateMethodCaller_DoubleTryParse_InvalidInput()
        {
            var method = typeof(double).GetMethod(nameof(double.TryParse), [typeof(string), typeof(double).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseDoubleDelegate>(method, typeof(object));

            var returned = (bool)caller("not_a_number", out double result);

            Assert.IsFalse(returned);
            Assert.AreEqual(0d, result);
        }

        [TestMethod]
        public void CreateMethodCaller_FloatTryParse_ValidInput()
        {
            var method = typeof(float).GetMethod(nameof(float.TryParse), [typeof(string), typeof(float).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseFloatDelegate>(method, typeof(object));

            var returned = (bool)caller("5", out float result);

            Assert.IsTrue(returned);
            Assert.AreEqual(5f, result);
        }

        [TestMethod]
        public void CreateMethodCaller_FloatTryParse_InvalidInput()
        {
            var method = typeof(float).GetMethod(nameof(float.TryParse), [typeof(string), typeof(float).MakeByRefType()]);

            var caller = DynamicMethodDelegateFactory.CreateMethodCaller<TryParseFloatDelegate>(method, typeof(object));

            var returned = (bool)caller("not_a_number", out float result);

            Assert.IsFalse(returned);
            Assert.AreEqual(0f, result);
        }

        #endregion

        #region Nested Types

        private sealed class SampleObject
        {
            public SampleObject() { }
        }

        private sealed class StringHolder
        {
            public StringHolder(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        #endregion
    }
}
