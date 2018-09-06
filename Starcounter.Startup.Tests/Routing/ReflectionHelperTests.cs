using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Starcounter.Startup.Routing;

namespace Starcounter.Startup.Tests.Routing
{
    public class ReflectionHelperTests
    {
        private class MoreThanOneMethod
        {
            [Mark]
            public static void One(){}
            [Mark]
            public static void Two(){}
        }

        [Test]
        public void ThrowsWhenMoreMultipleMethodsHaveAttribute()
        {
            AssertInspectingTypeThrows<MoreThanOneMethod>(StringsFormatted.ReflectionHelper_MultipleMethods(
                typeof(MarkAttribute), new[]
                {
                    typeof(MoreThanOneMethod).GetMethod(nameof(MoreThanOneMethod.One)),
                    typeof(MoreThanOneMethod).GetMethod(nameof(MoreThanOneMethod.Two)),
                }));
        }

        private class ReturnsString
        {
            [Mark]
            public static string MarkedMethod(int arg) => "";
        }

        [Test]
        public void ThrowsWhenReturnTypeDoesntMatch()
        {
            AssertInspectingTypeThrows<ReturnsString>(StringsFormatted.ReflectionHelper_WrongReturnType(
                typeof(MarkAttribute), typeof(int), typeof(ReturnsString).GetMethod(nameof(ReturnsString.MarkedMethod))));
        }

        private class ReturnsVoid
        {
            [Mark]
            public static void MarkedMethod(int arg){}
        }

        [Test]
        public void ThrowsWhenReturnTypeIsVoid()
        {
            AssertInspectingTypeThrows<ReturnsVoid>(StringsFormatted.ReflectionHelper_WrongReturnType(
                typeof(MarkAttribute), typeof(int), typeof(ReturnsVoid).GetMethod(nameof(ReturnsVoid.MarkedMethod))));
        }

        private class AcceptsNothing
        {
            [Mark]
            public static int MarkedMethod() => 0;
        }

        [Test]
        public void ThrowsWhenMethodHasNoParameters()
        {
            AssertInspectingTypeThrows<AcceptsNothing>(StringsFormatted.ReflectionHelper_MethodShouldAcceptOneParameter(
                typeof(MarkAttribute), typeof(int), typeof(AcceptsNothing).GetMethod(nameof(AcceptsNothing.MarkedMethod))));
        }

        private class NotStatic
        {
            [Mark]
            public int MarkedMethod(int arg) => 0;
        }

        [Test]
        public void ThrowsWhenMethodIsNotStatic()
        {
            AssertInspectingTypeThrows<NotStatic>(StringsFormatted.ReflectionHelper_MethodNotStatic(
                typeof(MarkAttribute), typeof(NotStatic).GetMethod(nameof(NotStatic.MarkedMethod))));
        }

        private class NotPublic
        {
            [Mark]
            internal static int MarkedMethod(int arg) => 0;
        }

        [Test]
        public void ThrowsWhenMethodIsNotPublic()
        {
            AssertInspectingTypeThrows<NotPublic>(StringsFormatted.ReflectionHelper_MethodNotPublic(
                typeof(MarkAttribute), typeof(NotPublic).GetMethod(nameof(NotPublic.MarkedMethod), BindingFlags.NonPublic|BindingFlags.Static)));
        }

        private class NoMarkedMethods
        {
            public static int NotMarked(int arg) => 0;
        }

        [Test]
        public void ReturnsNullWhenNoMethodsAreMarked()
        {
            AssertInspectingTypeReturns<NoMarkedMethods>(null);
        }

        private class ValidMarkedMethod
        {
            [Mark]
            public static int Marked(int arg) => 0;
        }

        [Test]
        public void ReturnsMethodInfoWhenMarkedMethodFitsAllCriteria()
        {
            AssertInspectingTypeReturns<ValidMarkedMethod>(typeof(ValidMarkedMethod).GetMethod(nameof(ValidMarkedMethod.Marked)));
        }

        private class ValidMarkedMethodWithMoreParameters
        {
            [Mark]
            public static int Marked(int arg, string arg2) => 0;
        }

        [Test]
        public void ReturnsMethodInfoWhenMarkedMethodFitsAllCriteriaAndHasMoreParameters()
        {
            AssertInspectingTypeReturns<ValidMarkedMethodWithMoreParameters>(typeof(ValidMarkedMethodWithMoreParameters).GetMethod(nameof(ValidMarkedMethodWithMoreParameters.Marked)));
        }

        private void AssertInspectingTypeThrows<TTargetType>(string expectedMessage)
        {
            new Action(() => ReflectionHelper.GetStaticMethodWithAttribute(
                    typeof(TTargetType),
                    typeof(MarkAttribute),
                    typeof(int),
                    typeof(int)))
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage(StringsFormatted.ReflectionHelper_InvalidApplicationOfAttribute(typeof(MarkAttribute), typeof(TTargetType), expectedMessage));
        }

        private void AssertInspectingTypeReturns<TTargetType>(MethodInfo expected)
        {
            ReflectionHelper.GetStaticMethodWithAttribute(
                    typeof(TTargetType),
                    typeof(MarkAttribute),
                    typeof(int),
                    typeof(int))
                .Should()
                .BeSameAs(expected);
        }

        [AttributeUsage(AttributeTargets.Method)]
        private sealed class MarkAttribute: Attribute { }
    }

}