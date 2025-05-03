/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta.Voice.Hub.Utilities
{
    internal static class ReflectionUtils
    {
        private const string NAMESPACE_PREFIX = "Meta";

        private static bool IsValidNamespace(Type type) =>
            type.Namespace != null && type.Namespace.StartsWith(NAMESPACE_PREFIX);

        private static List<Type> GetTypes(Func<Type, bool> isValid)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch
                    {
                        return new Type[] { };
                    }
                })
                .Where(IsValidNamespace)
                .Where(isValid)
                .ToList();
        }

        internal static List<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            var attributeType = typeof(T);
            return GetTypes(type => type.GetCustomAttributes(attributeType, false).Length > 0);
        }
    }
}
