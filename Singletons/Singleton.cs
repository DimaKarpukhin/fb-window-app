﻿namespace Singletons
{
    using System;
    using System.Reflection;

    // Copyright (c) DevInstinct Inc. All rights reserved.
    // http://devinstinct.com
    // Code author: Martin Lapierre
    // You cannot remove this copyright and notice.
    // This source file is part of the DevInstinct Application Framework.
    // You can redistribute the DevInstinct Application Framework in compiled, obfuscated form.
    // You cannot redistribute its source files without the written consent of DevInstinct Inc.
    // You can freely use and redistribute THIS source file: Singleton.cs

    /// <summary>
    /// Manages the single instance of a class.
    /// </summary>
    /// <typeparam name="T">Type of the singleton class.</typeparam>
    public static class Singleton<T>
        where T : class
    {
        /// <summary>
        /// The single instance of the target class.
        /// </summary>
        /// <remarks>
        /// The volatile keyword makes sure to remove any compiler optimization that could make concurrent 
        /// threads reach a race condition with the double-checked lock pattern used in the Instance property.
        /// See http://www.bluebytesoftware.com/blog/PermaLink,guid,543d89ad-8d57-4a51-b7c9-a821e3992bf6.aspx
        /// </remarks>
        private static volatile T s_Instance;

        /// <summary>
        /// The dummy object used for locking.
        /// </summary>
        private static object s_LockObj = new object();

        /// <summary>
        /// Type-initializer to prevent type to be marked with beforefieldinit.
        /// </summary>
        /// <remarks>
        /// This simply makes sure that static fields initialization occurs 
        /// when Instance is called the first time and not before.
        /// </remarks>
        static Singleton()
        {
        }

        /// <summary>
        /// Gets the single instance of the class.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_LockObj)
                    {
                        if (s_Instance == null)
                        {
                            ConstructorInfo constructor = null;

                            try
                            {
                                /// Binding flags exclude public and static constructors.
                                constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
                            }
                            catch (Exception exception)
                            {
                                throw new Exception(null, exception);
                            }

                            /// Also exclude internal constructors.
                            if (constructor == null || constructor.IsAssembly) 
                            {
                                throw new Exception(string.Format("A private or protected constructor is missing for '{0}'.", typeof(T).Name));
                            }

                            s_Instance = constructor.Invoke(null) as T;
                        }
                    }
                }

                return s_Instance;
            }
        }
    }
}