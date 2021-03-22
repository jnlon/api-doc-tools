﻿using mdoc.Test.SampleClasses;
using Mono.Cecil;
using Mono.Documentation.Updater;
using Mono.Documentation.Updater.Formatters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mdoc.Test
{
    [TestFixture()]
    public class AttributeValueFormatterTest : BasicTests
    {
        [TestCase("PropertyTypeType", "typeof(Mono.Cecil.TypeReference)")]
        [TestCase("PropertyTypeTypeWithNull", "null")]
        [TestCase("PropertyBoolType", "true")]
        [TestCase("PropertySByteType", SByte.MinValue)]
        [TestCase("PropertyByteType", Byte.MaxValue)]
        [TestCase("PropertyInt16Type", Int16.MinValue)]
        [TestCase("PropertyUInt16Type", UInt16.MaxValue)]
        [TestCase("PropertyInt32Type", Int32.MinValue)]
        [TestCase("PropertyUInt32Type", UInt32.MaxValue)]
        [TestCase("PropertyInt64Type", Int64.MinValue)]
        [TestCase("PropertyUInt64Type", UInt64.MaxValue)]
        [TestCase("PropertySingleType", Single.MinValue)]
        [TestCase("PropertyDoubleType", Double.MinValue)]
        [TestCase("PropertyCharType", "'C'")]
        [TestCase("PropertyStringType", "\"This is a string argument.\"")]
        [TestCase("PropertyStringTypeWithEmptyString", "\"\"")]
        [TestCase("PropertyStringTypeWithNull", "null")]
        [TestCase("PropertyEnumType", "System.ConsoleColor.Red")]
        [TestCase("PropertyEnumTypeWithUnknownValue", "(System.ConsoleColor) 2147483647")]
        [TestCase("PropertyFlagsEnumType", "System.AttributeTargets.Class | System.AttributeTargets.Enum")]
        [TestCase("PropertyFlagsEnumTypeWithAllValue", "System.AttributeTargets.All")]
        [TestCase("PropertyArrayOfIntType", "new System.Int32[] { 0, 0, 7 }")]
        [TestCase("PropertyArrayOfIntTypeWithNull", "null")]
        [TestCase("PropertyObjectWithNull", "null")]
        [TestCase("PropertyObjectWithBoolType", "true")]
        [TestCase("PropertyObjectWithSByteType", SByte.MinValue)]
        [TestCase("PropertyObjectWithByteType", Byte.MaxValue)]
        [TestCase("PropertyObjectWithInt16Type", Int16.MinValue)]
        [TestCase("PropertyObjectWithUInt16Type", UInt16.MaxValue)]
        [TestCase("PropertyObjectWithInt32Type", Int32.MinValue)]
        [TestCase("PropertyObjectWithUInt32Type", UInt32.MaxValue)]
        [TestCase("PropertyObjectWithInt64Type", Int64.MinValue)]
        [TestCase("PropertyObjectWithUInt64Type", UInt64.MaxValue)]
        [TestCase("PropertyObjectWithSingleType", Single.MinValue)]
        [TestCase("PropertyObjectWithDoubleType", Double.MinValue)]
        [TestCase("PropertyObjectWithCharType", "'C'")]
        [TestCase("PropertyObjectWithStringType", "\"This is a string argument.\"")]
        [TestCase("PropertyObjectWithStringTypeAndEmptyString", "\"\"")]
        [TestCase("PropertyObjectWithEnumType", "System.ConsoleColor.Red")]
        [TestCase("PropertyObjectWithEnumTypeAndUnknownValue", "(System.ConsoleColor) 2147483647")]
        [TestCase("PropertyObjectWithFlagsEnumType", "System.AttributeTargets.Class | System.AttributeTargets.Enum")]
        [TestCase("PropertyObjectWithFlagsEnumTypeAndAllValue", "System.AttributeTargets.All")]
        [TestCase("PropertyObjectWithArrayOfIntType", "new System.Int32[] { 0, 0, 7 }")]
        public void TestFormatValueWithDifferentTypes(string methodName, object argumentValue)
        {
            TestAttributeValueFormatter(methodName, argumentValue);
        }

        private void TestAttributeValueFormatter(string memberName, object expectedValues)
        {
            TestAttributeValueFormatter(typeof(SomeAttribute), memberName, expectedValues);
        }

        private void TestAttributeValueFormatter(Type type, string memberName, object expectedValue)
        {
            var (argumentType, argumentValue) = GetAttributeArguments(type, memberName);

            var attributeFormatter = new AttributeFormatter();
            var formatValue = attributeFormatter.MakeAttributesValueString(argumentValue, argumentType);

            Assert.AreEqual(expectedValue.ToString(), formatValue);
        }

        private (TypeReference argumentType, object argumentValue) GetAttributeArguments(Type type, string memberName)
        {
            var methodDefinition = GetMethod(type, memberName);
            var methodAttribute = AttributeFormatter.GetCustomAttributes(methodDefinition).First();
            CustomAttribute customAttribute = methodAttribute.Item1;

            var customAttributeList = new List<(TypeReference, object)>();
            for (int i = 0; i < customAttribute.ConstructorArguments.Count; ++i)
            {
                customAttributeList.Add((customAttribute.ConstructorArguments[i].Type, customAttribute.ConstructorArguments[i].Value));
            }

            foreach (var item in GetAttributeArgumentsFromFieldsAndProperties(customAttribute))
            {
                customAttributeList.Add((item.argumentType, item.argumentValue));
            }

            return customAttributeList.First();
        }

        private IEnumerable<(string argumentName, TypeReference argumentType, object argumentValue)> GetAttributeArgumentsFromFieldsAndProperties(CustomAttribute customAttribute)
        {
            return (from namedArg in customAttribute.Fields
                    select (namedArg.Name, namedArg.Argument.Type, namedArg.Argument.Value))
                    .Concat(from namedArg in customAttribute.Properties
                            select (namedArg.Name, namedArg.Argument.Type, namedArg.Argument.Value))
                    .OrderBy(v => v.Name);
        }
    }
}