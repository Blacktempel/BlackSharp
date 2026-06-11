/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2026 Florian K.
 *
 */

using System.Reflection;
using System.Reflection.Emit;

namespace BlackSharp.Core.Reflection.Emit
{
#if !NETSTANDARD2_0_OR_GREATER

    public delegate TRet DynamicMethodDelegate<TRet>(object target, params object[] args);
    public delegate void DynamicMethodDelegate(object target, params object[] args);

    /// <summary>
    /// Factory to create dynamic methods from <see cref="MethodInfo"/>.
    /// </summary>
    /// <seealso href="https://stackoverflow.com/questions/29131117/using-ilgenerator-emit-to-call-a-method-in-another-assembly-that-has-an-out-para"/>
    public class DynamicMethodDelegateFactory
    {
        #region Public

        public static TDelegate CreateMethodCaller<TDelegate>(MethodInfo method, Type returnType = null, Type instanceType = null, params Type[] parameterTypes)
            where TDelegate : class
        {
            returnType = returnType ?? method.ReturnType;

            bool isStatic      = method.IsStatic;
            bool hasReturnType = returnType != typeof(void);
            
            var parameters = method.GetParameters();
            var args = (parameterTypes.Length > 0 ? parameterTypes : parameters.Select(p => p.ParameterType)).ToList();

            if (!isStatic)
            {
                args.Insert(0, instanceType ?? method.DeclaringType);
            }

            var dyn = new DynamicMethod
                (
                    method.Name,
                    returnType,
                    args.ToArray(),
                    typeof(DynamicMethodDelegateFactory),
                    true
                );

            //Add parameters to dynamic method
            for (int i = 0; i < parameters.Length; ++i)
            {
                dyn.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
            }

            var il = dyn.GetILGenerator();

            //If method requires instance of object, load the instance
            if (!isStatic)
            {
                il.Emit(OpCodes.Ldarg, 0);
            }

            //Load parameters onto elevation stack
            for (int i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];

                //If parameter is not out parameter, load it
                if (!param.IsOut)
                    il.Emit(OpCodes.Ldarg, isStatic ? i : i + 1);
                if (param.ParameterType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, param.ParameterType);
            }

            //Declare local variables for out parameters
            var locals = new LocalBuilder[parameters.Length + 1]; //+ 1 for possible return value
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].IsOut)
                {
                    locals[i] = il.DeclareLocal(parameters[i].ParameterType.GetElementType());
                    il.Emit(OpCodes.Ldloca, locals[i].LocalIndex);
                }
            }

            //Also declare variable for return type, if there's any
            if (hasReturnType)
            {
                locals[locals.Length - 1] = il.DeclareLocal(returnType);
            }

            //Call the method
            if (method.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }

            //Method returned something
            if (hasReturnType)
            {
                //Specified return type might differ (object) so box is needed
                if (returnType != method.ReturnType)
                {
                    if (method.ReturnType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, method.ReturnType);
                    }
                }

                //Store returned value from method
                il.Emit(OpCodes.Stloc, locals[locals.Length - 1].LocalIndex);
            }

            //Load out parameters, load local & store result
            for (int i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];
                if (param.IsOut || param.ParameterType.IsByRef)
                {
                    //Load parameter
                    il.Emit(OpCodes.Ldarg, isStatic ? i : i + 1);

                    //Load local variable
                    il.Emit(OpCodes.Ldloc, locals[i].LocalIndex);

                    //Perform assignment
                    var elemType = param.ParameterType.GetElementType();
                    if (elemType.IsValueType)
                    {
                        il.Emit(OpCodes.Stobj, elemType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stind_Ref);
                    }
                }
            }

            //If return type exists, load the value returned by method
            if (hasReturnType)
            {
                il.Emit(OpCodes.Ldloc, locals[locals.Length - 1].LocalIndex);
            }

            //Return
            il.Emit(OpCodes.Ret);

            //Create and return delegate to call dynamic method
            return dyn.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static TDelegate CreateMethodAllocator<TDelegate>(Type typeToCreate, params Type[] argumentTypes)
            where TDelegate : class
        {
            var ci = typeToCreate.GetConstructor(argumentTypes);

            var parameters = ci.GetParameters();
            var args = parameters.Select(p => p.ParameterType).ToArray();

            var dyn = new DynamicMethod
                (
                    "DynAllocate" + ci.DeclaringType.Name,
                    //ci.DeclaringType,
                    typeof(object),
                    args,
                    typeof(DynamicMethodDelegateFactory),
                    true
                );

            //Add parameters to dynamic method
            for (int i = 0; i < parameters.Length; ++i)
            {
                dyn.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
            }

            var il = dyn.GetILGenerator();

            //Load parameters onto elevation stack
            for (int i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];

                //If parameter is not out parameter, load it
                if (!param.IsOut)
                {
                    il.Emit(OpCodes.Ldarg, i);
                }

                if (param.ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, param.ParameterType);
                }
            }

            //Create new instance
            il.Emit(OpCodes.Newobj, ci);

            //Return new instance
            il.Emit(OpCodes.Ret);

            //Create and return delegate to call dynamic method
            return dyn.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        #endregion
    }

#endif
}
