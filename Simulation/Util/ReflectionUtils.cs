﻿using System;
using System.Reflection;

namespace Simulation.Util
{
    public class ReflectionUtils
    {
        private static BindingFlags Flags = BindingFlags.Instance
                                            | BindingFlags.GetProperty
                                            | BindingFlags.SetProperty
                                            | BindingFlags.GetField
                                            | BindingFlags.SetField
                                            | BindingFlags.NonPublic
                                            | BindingFlags.Public;

        public static void SetMemberValue(Type type, object instance, string name, object newValue)
        {
            var field = type.GetField(name, Flags);

            if(field != null)
            {
                field.SetValue(instance, newValue);
            }
            else
            {
                var property = type.GetProperty(name, Flags);

                if (property == null) throw new Exception("Member " + name + " not found!");

                property.SetValue(instance, newValue);
            }
        }

        public static object GetMemberValue(Type type, object instance, string name)
        {
            var field = type.GetField(name, Flags);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else
            {
                var property = type.GetProperty(name, Flags);

                if (property == null) throw new Exception("Member " + name + " not found!");

                return property.GetValue(instance);
            }
        }

        public static Type GetMemberType(Type type, string name)
        {
            var field = type.GetField(name, Flags);

            if (field != null)
            {
                return field.FieldType;
            }
            else
            {
                var property = type.GetProperty(name, Flags);

                if (property == null) throw new Exception("Member " + name + " not found!");

                return property.PropertyType;
            }
        }

        public static void SetPrivateField(Type type, object instance, string name, object newValue)
        {
            var field = type.GetField(name, Flags);

            if (field == null) throw new Exception("Field " + name + " not found!");

            field.SetValue(instance, newValue);
        }

        public static object GetPrivateField(Type type, object instance, string name)
        {
            var field = type.GetField(name, Flags);

            if (field == null) throw new Exception("Field " + name + " not found!");

            return field.GetValue(instance);
        }

        public static Type GetFieldType(Type type, string name)
        {
            return type.GetField(name, Flags).FieldType;
        }

        public static void SetPrivateProperty(Type type, object instance, string name, object newValue)
        {
            var property = type.GetProperty(name, Flags);

            if (property == null) throw new Exception("Property " + name + " not found!");

            property.SetValue(instance, newValue);
        }

        public static object GetPrivateProperty(Type type, object instance, string name)
        {
            var property = type.GetProperty(name, Flags);

            if (property == null) throw new Exception("Property " + name + " not found!");

            return property.GetValue(instance);
        }

        public static Type GetPropertyType(Type type, string name)
        {
            return type.GetProperty(name, Flags).PropertyType;
        }

        public static T CallPrivateConstructor<T>()
        {
            return CallPrivateConstructor<T>(null, null);
        }

        public static T CallPrivateConstructor<T>(Type[] paramTypes, object[] paramValues)
        {
            if (paramTypes == null)
                paramTypes = new Type[0];

            ConstructorInfo ci = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, paramTypes, null);

            return (T)ci.Invoke(paramValues);
        }
    }
}
