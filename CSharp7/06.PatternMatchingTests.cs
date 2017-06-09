using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp7
{
    public abstract class Storage { }
    public class Dvd : Storage
    {
        public Boolean IsInserted { get; internal set; }

        internal void Eject()
        {
            throw new NotImplementedException();
        }

    }
    public class HardDrive : Storage { }
    public class FloppyDrive : Storage { }
    public class UsbKey : Storage
    {
        public Boolean IsPluggedIn { get; internal set; }

        internal void Unload()
        {
            throw new NotImplementedException();
        }
    }

    public class PatternMatchingTests
    {
        [TestClass]
        public class IsOperatorPatternMatchingExamples
        {
            [TestMethod]
            public void IsOperatorWithInt32()
            {
                int? number = 5;
                if (number is int x)
                {
                    Assert.AreEqual<int>(5, x);
                }
                else
                    throw new Exception();
            }
            [TestMethod]
            public void IsOperatorWithInt32SetToNull()
            {
                int? number = null;
                if (number is int x)
                {
                    Assert.Fail();
                }
            }

            [TestMethod]
            public void IsAsAnEqualityOperator()
            {
                Storage storage = null;
                Assert.IsTrue(storage is null);
                storage = new UsbKey();
                Assert.IsFalse(storage is null);

                string text1 = "Inigo Montoya";
                Assert.IsTrue(text1 is "Inigo Montoya");
            }

            public void Eject(Storage storage)
            {
                if (storage is null)
                {
                    throw new ArgumentNullException(nameof(storage));
                }
                if ((storage is UsbKey usbDrive) &&
                    usbDrive.IsPluggedIn)
                {
                    usbDrive.Unload();
                    Console.WriteLine("USB Drive Unloaded.");
                }
                else if (storage is HardDrive hardDrive)
                {
                    throw new InvalidOperationException();
                }
                else if (storage is Dvd dvd && dvd.IsInserted)
                {
                    dvd.Eject();
                }
                else throw new NotImplementedException();
            }

            [TestMethod]
            public void IsArgument_Variable_FailsCompile()
            {
                CompilerAssert.StatementsFailCompilation(
                    @"Guid guid1 = new Guid();
                    Guid guid2 = new Guid(guid1.ToByteArray());
                    Assert.IsTrue(guid1 is guid2);",
                    @"Error CS0150: A constant value is expected
Warning CS1701: Assuming assembly reference 'mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' used by 'Microsoft.VisualStudio.QualityTools.UnitTestFramework' matches identity 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' of 'mscorlib', you may need to supply runtime policy
Warning CS1701: Assuming assembly reference 'mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' used by 'Microsoft.VisualStudio.QualityTools.UnitTestFramework' matches identity 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' of 'mscorlib', you may need to supply runtime policy"
                    );
            }
        }

        [TestClass]
        public class SwitchPatternMatchingExamples
        {
            public void Eject(Storage storage)
            {
                switch(storage)
                {
                    // case Storage tempStorage:
                    //    throw new Exception();
                    //    break;
                    case UsbKey usbKey when usbKey.IsPluggedIn:
                        usbKey.Unload();
                        Console.WriteLine("USB Drive Unloaded.");
                        break;
                    case Dvd dvd when dvd.IsInserted:
                        dvd.Eject();
                        break;
                    case HardDrive hardDrive:
                        throw new InvalidOperationException();
                    case null:
                    default:
                        throw new ArgumentNullException();
                }
            }

            [TestMethod]
            public void SwitchStatement_DefaultFirst_MatchesLast()
            {
                Storage storage = null;
                switch (storage)
                {
                    default:
                        throw new InvalidOperationException();
                    case null:
                        break;
                }
            }

            [TestMethod]
            public void SwitchPatternMatchingScope()
            {
                CompilerAssert.StatementsFailCompilation(
                    @"
                    object o = new object();
                    switch(o)
                    {   
                        case int number:
                            return;
                    }
                    number = 42;
                    ",
                    "Error CS0103: The name 'number' does not exist in the current context"
                    );
            }
            [TestMethod]
            public void SwitchPatternMatching_BaseClassCaseFirst_Fails()
            {
                CompilerAssert.StatementsFailCompilation(
                    @"
                    Object obj = null;
                    switch(obj)
                    {                      
                        case Exception exception:
                            break;
                        case ArgumentException argumentException:
                            break;
                    }
                    ",
                    @"Error CS8120: The switch case has already been handled by a previous case.",
                "Warning CS0162: Unreachable code detected"
                    );
            }        }
    }

    class BeforePatternMatchingExamples
    {
        public void Eject(Storage storage)
        {
            if (storage is null)
            {
                throw new ArgumentNullException();
            }
            if (storage is UsbKey)
            {
                UsbKey usbKey = (UsbKey)storage;
                if (usbKey.IsPluggedIn)
                {
                    usbKey.Unload();
                    Console.WriteLine("USB Drive Unloaded.");
                }
                else throw new NotImplementedException();
            }
            else if (storage is HardDrive)
            {
                throw new InvalidOperationException();
            }
            else if (storage is Dvd)
            {
                Dvd dvd = (Dvd)storage;
                if (dvd.IsInserted) { dvd.Eject(); }
                else throw new NotImplementedException();
            }
            else throw new NotImplementedException();
        }

    }
}

