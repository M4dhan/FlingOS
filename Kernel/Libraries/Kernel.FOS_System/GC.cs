﻿#region LICENSE
// ---------------------------------- LICENSE ---------------------------------- //
//
//    Fling OS - The educational operating system
//    Copyright (C) 2015 Edward Nutting
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//  Project owner: 
//		Email: edwardnutting@outlook.com
//		For paper mail address, please contact via email for details.
//
// ------------------------------------------------------------------------------ //
#endregion
    
//#define GC_TRACE

using System;
using Kernel.FOS_System.Processes.Synchronisation;

namespace Kernel.FOS_System
{
    /// <summary>
    /// The garbage collector.
    /// </summary>
    /// <remarks>
    /// Make sure all methods that the GC calls are marked with [Compiler.NoGC] (including
    /// get-set property methods! Apply the attribute to the get/set keywords not the property
    /// declaration (/name).
    /// </remarks>
    public static unsafe class GC
    {
        //TODO: GC needs an object reference tree to do a thorough scan to find reference loops

        /// <summary>
        /// Whether the GC has been initialised yet or not.
        /// Used to prevent the GC running before it has been initialised properly.
        /// </summary>
        [Drivers.Compiler.Attributes.Group(Name = "IsolatedKernel_FOS_System")]
        public static bool Enabled = false;

        [Drivers.Compiler.Attributes.Group(Name = "IsolatedKernel_FOS_System")]
        public static bool UseCurrentState = false;

        private static GCState state;
        [Drivers.Compiler.Attributes.Group(Name="IsolatedKernel_FOS_System")]
        private static GCState kernel_state;
        public static GCState State
        {
            [Drivers.Compiler.Attributes.NoDebug]
            [Drivers.Compiler.Attributes.NoGC]
            get
            {
                if (UseCurrentState)
                {
                    return state;
                }
                else
                {
                    return kernel_state;
                }
            }
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (UseCurrentState)
                {
                    state = value;
                }
                else
                {
                    kernel_state = value;
                }
            }
        }
        private static bool StateInitialised
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                return State != null;
            }
        }
        
        public static bool OutputTrace
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.OutputTrace;
                }
                else
                {
                    return false;
                }
            }
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (StateInitialised)
                {
                    State.OutputTrace = value;
                }
            }
        }
        public static bool InsideGC
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.InsideGC;
                }
                else
                {
                    return false;
                }
            }
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (StateInitialised)
                {
                    State.InsideGC = value;
                }
            }
        }
        public static bool AccessLockInitialised
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.AccessLockInitialised;
                }
                else
                {
                    return false;
                }
            }
        }
        public static SpinLock AccessLock
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.AccessLock;
                }
                return null;
            }
        }

        public static int NumObjs
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.NumObjs;
                }
                else
                {
                    return 0;
                }
            }
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (StateInitialised)
                {
                    State.NumObjs = value;
                }
            }
        }
        public static int NumStrings
        {
            [Drivers.Compiler.Attributes.NoGC]
            get
            {
                if (StateInitialised)
                {
                    return State.NumStrings;
                }
                else
                {
                    return 0;
                }
            }
            [Drivers.Compiler.Attributes.NoGC]
            set
            {
                if (StateInitialised)
                {
                    State.NumStrings = value;
                }
            }
        }

        public static FOS_System.String lastEnabler
        {
            [Drivers.Compiler.Attributes.NoDebug]
            [Drivers.Compiler.Attributes.NoGC]
            get
            {
                if (StateInitialised)
                {
                    return State.lastEnabler;
                }
                else
                {
                    return "";
                }
            }
            [Drivers.Compiler.Attributes.NoDebug]
            [Drivers.Compiler.Attributes.NoGC]
            set
            {
                if (StateInitialised)
                {
                    State.lastEnabler = value;
                }
            }
        }
        public static FOS_System.String lastDisabler
        {
            [Drivers.Compiler.Attributes.NoDebug]
            [Drivers.Compiler.Attributes.NoGC]
            get;
            //{
            //    if (StateInitialised)
            //    {
            //        return state.lastDisabler;
            //    }
            //    else
            //    {
            //        return "";
            //    }
            //}
            [Drivers.Compiler.Attributes.NoDebug]
            [Drivers.Compiler.Attributes.NoGC]
            set;
            //{
            //    if (StateInitialised)
            //    {
            //        state.lastDisabler = value;
            //    }
            //}
        }
        public static FOS_System.String lastLocker
        {
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.lastLocker;
                }
                else
                {
                    return "";
                }
            }
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (StateInitialised)
                {
                    State.lastLocker = value;
                }
            }
        }

        public static ObjectToCleanup* CleanupList
        {
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            get
            {
                if (StateInitialised)
                {
                    return State.CleanupList;
                }
                else
                {
                    return null;
                }
            }
            [Drivers.Compiler.Attributes.NoGC]
            [Drivers.Compiler.Attributes.NoDebug]
            set
            {
                if (StateInitialised)
                {
                    State.CleanupList = value;
                }
            }
        }

        /// <summary>
        /// Initialises the GC.
        /// </summary>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void Init()
        {
            ExceptionMethods.state = ExceptionMethods.kernel_state = (ExceptionState*)Heap.AllocZeroed((uint)sizeof(ExceptionState), "GC()");
            
            Enabled = true;

            Heap.AccessLock = new SpinLock();
            Heap.AccessLockInitialised = true;

            GCState newState1 = new GCState();
            kernel_state = newState1;
            newState1.AccessLock = new SpinLock();
            newState1.AccessLockInitialised = true;

            GCState newState2 = new GCState();
            state = newState2;
            newState2.AccessLock = new SpinLock();
            newState2.AccessLockInitialised = true;
        }

        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void Enable(FOS_System.String caller)
        {
            //BasicConsole.Write(caller);
            //BasicConsole.WriteLine(" enabling GC.");
            //BasicConsole.DelayOutput(2);

            lastEnabler = caller;
            GC.Enabled = true;
        }
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void Disable(FOS_System.String caller)
        {
            //BasicConsole.Write(caller);
            //BasicConsole.WriteLine(" disabling GC.");
            //BasicConsole.DelayOutput(2);

            lastDisabler = caller;
            GC.Enabled = false;
        }

        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        private static void EnterCritical(FOS_System.String caller)
        {
            //BasicConsole.WriteLine("Entering critical section...");
            if (AccessLockInitialised)
            {
                if (AccessLock == null)
                {
                    BasicConsole.WriteLine("GCAccessLock is initialised but null?!");
                    BasicConsole.DelayOutput(10);
                }
                else
                {
#if GC_TRACE
                    if (AccessLock.Locked && OutputTrace)
                    {
                        BasicConsole.SetTextColour(BasicConsole.warning_colour);
                        BasicConsole.WriteLine("Warning: GC about to try to re-enter spin lock...");
                        BasicConsole.Write("Enter lock caller: ");
                        BasicConsole.WriteLine(caller);
                        BasicConsole.Write("Previous caller: ");
                        BasicConsole.WriteLine(lastLocker);
                        BasicConsole.SetTextColour(BasicConsole.default_colour);

                        //ExceptionMethods.PrintStack();
                        //ExceptionMethods.PrintStackTrace();
                    }
#endif

                    AccessLock.Enter();

#if GC_TRACE
                    lastLocker = caller;
#endif
                }
            }
            //else
            //{
            //    BasicConsole.WriteLine("GCAccessLock not initialised - ignoring lock conditions.");
            //    BasicConsole.DelayOutput(5);
            //}
        }
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        private static void ExitCritical()
        {
            //BasicConsole.WriteLine("Exiting critical section...");
            if (AccessLockInitialised)
            {
                if (AccessLock == null)
                {
                    BasicConsole.WriteLine("GCAccessLock is initialised but null?!");
                    BasicConsole.DelayOutput(10);
                }
                else
                {
                    AccessLock.Exit();
                }
            }
            //else
            //{
            //    BasicConsole.WriteLine("GCAccessLock not initialised - ignoring lock conditions.");
            //    BasicConsole.DelayOutput(5);
            //}
        }

        /// <summary>
        /// Creates a new object of specified type (but does not call the default constructor).
        /// </summary>
        /// <param name="theType">The type of object to create.</param>
        /// <returns>A pointer to the new object in memory.</returns>
        [Drivers.Compiler.Attributes.NewObjMethod]
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void* NewObj(FOS_System.Type theType)
        {
            if (!Enabled)
            {
                BasicConsole.SetTextColour(BasicConsole.warning_colour);
                BasicConsole.WriteLine("Warning! GC returning null pointer because GC not enabled.");
                BasicConsole.Write("Last disabler: ");
                BasicConsole.WriteLine(lastDisabler);
                BasicConsole.DelayOutput(10);
                BasicConsole.SetTextColour(BasicConsole.default_colour);

                return null;
            }

#if GC_TRACE
            if (OutputTrace)
            {
                BasicConsole.WriteLine("NewObj");
            }
#endif

            EnterCritical("NewObj");

            try
            {
                InsideGC = true;

                //Alloc space for GC header that prefixes object data
                //Alloc space for new object

                uint totalSize = theType.Size;
                totalSize += (uint)sizeof(GCHeader);

                GCHeader* newObjPtr = (GCHeader*)Heap.AllocZeroed(totalSize, "GC : NewObject");

                if ((UInt32)newObjPtr == 0)
                {
                    InsideGC = false;

                    BasicConsole.SetTextColour(BasicConsole.error_colour);
                    BasicConsole.WriteLine("Error! GC can't create a new object because the heap returned a null pointer.");
                    BasicConsole.DelayOutput(10);
                    BasicConsole.SetTextColour(BasicConsole.default_colour);

                    return null;
                }

                NumObjs++;

                //Initialise the GCHeader
                SetSignature(newObjPtr);
                newObjPtr->RefCount = 1;
                //Initialise the object _Type field
                FOS_System.Object newObj = (FOS_System.Object)Utilities.ObjectUtilities.GetObject(newObjPtr + 1);
                newObj._type = theType;

                //Move past GCHeader
                byte* newObjBytePtr = (byte*)(newObjPtr + 1);

                InsideGC = false;

                return newObjBytePtr;
            }
            finally
            {
                ExitCritical();
            }
        }

        /// <summary>
        /// Creates a new array with specified element type (but does not call the default constructor).
        /// </summary>
        /// <remarks>"length" param placed first so that calling NewArr method is simple
        /// with regards to pushing params onto the stack.</remarks>
        /// <param name="length">The length of the array to create.</param>
        /// <param name="elemType">The type of element in the array to create.</param>
        /// <returns>A pointer to the new array in memory.</returns>
        [Drivers.Compiler.Attributes.NewArrMethod]
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void* NewArr(int length, FOS_System.Type elemType)
        {
            if (!Enabled)
            {
                BasicConsole.SetTextColour(BasicConsole.warning_colour);
                BasicConsole.WriteLine("Warning! GC returning null pointer because GC not enabled.");
                BasicConsole.Write("Last disabler: ");
                BasicConsole.WriteLine(lastDisabler);
                BasicConsole.DelayOutput(10);
                BasicConsole.SetTextColour(BasicConsole.default_colour);

                return null;
            }

#if GC_TRACE
            if (OutputTrace)
            {
                BasicConsole.WriteLine("NewArr");
            }
#endif

            EnterCritical("NewArr");

            try
            {

                if (length < 0)
                {
                    ExceptionMethods.Throw_OverflowException();
                }

                InsideGC = true;

                //Alloc space for GC header that prefixes object data
                //Alloc space for new array object
                //Alloc space for new array elems

                uint totalSize = ((FOS_System.Type)typeof(FOS_System.Array)).Size;
                if (elemType.IsValueType)
                {
                    totalSize += elemType.Size * (uint)length;
                }
                else
                {
                    totalSize += elemType.StackSize * (uint)length;
                }
                totalSize += (uint)sizeof(GCHeader);

                GCHeader* newObjPtr = (GCHeader*)Heap.AllocZeroed(totalSize, "GC : NewArray");

                if ((UInt32)newObjPtr == 0)
                {
                    InsideGC = false;

                    BasicConsole.SetTextColour(BasicConsole.error_colour);
                    BasicConsole.WriteLine("Error! GC can't create a new array because the heap returned a null pointer.");
                    BasicConsole.DelayOutput(10);
                    BasicConsole.SetTextColour(BasicConsole.default_colour);

                    return null;
                }

                NumObjs++;

                //Initialise the GCHeader
                SetSignature(newObjPtr);
                newObjPtr->RefCount = 1;

                FOS_System.Array newArr = (FOS_System.Array)Utilities.ObjectUtilities.GetObject(newObjPtr + 1);
                newArr._type = (FOS_System.Type)typeof(FOS_System.Array);
                newArr.length = length;
                newArr.elemType = elemType;

                //Move past GCHeader
                byte* newObjBytePtr = (byte*)(newObjPtr + 1);

                InsideGC = false;

                return newObjBytePtr;
            }
            finally
            {
                ExitCritical();
            }
        }

        /// <summary>
        /// DO NOT CALL DIRECTLY. Use FOS_System.String.New
        /// Creates a new string with specified length (but does not call the default constructor).
        /// </summary>
        /// <param name="length">The length of the string to create.</param>
        /// <returns>A pointer to the new string in memory.</returns>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void* NewString(int length)
        {
            if (!Enabled)
            {
                BasicConsole.SetTextColour(BasicConsole.warning_colour);
                BasicConsole.WriteLine("Warning! GC returning null pointer because GC not enabled.");
                BasicConsole.Write("Last disabler: ");
                BasicConsole.WriteLine(lastDisabler);
                BasicConsole.DelayOutput(10);
                BasicConsole.SetTextColour(BasicConsole.default_colour);

                return null;
            }

#if GC_TRACE
            if (OutputTrace)
            {
                BasicConsole.WriteLine("NewString");
            }
#endif

            EnterCritical("NewString");

            try
            {

                if (length < 0)
                {
                    BasicConsole.SetTextColour(BasicConsole.error_colour);
                    BasicConsole.WriteLine("Error! GC can't create a new string because \"length\" is less than 0.");
                    BasicConsole.DelayOutput(5);
                    BasicConsole.SetTextColour(BasicConsole.default_colour);

                    ExceptionMethods.Throw_OverflowException();
                }

                InsideGC = true;

                //Alloc space for GC header that prefixes object data
                //Alloc space for new string object
                //Alloc space for new string chars

                uint totalSize = ((FOS_System.Type)typeof(FOS_System.String)).Size;
                totalSize += /*char size in bytes*/2 * (uint)length;
                totalSize += (uint)sizeof(GCHeader);

                GCHeader* newObjPtr = (GCHeader*)Heap.AllocZeroed(totalSize, "GC : NewString");

                if ((UInt32)newObjPtr == 0)
                {
                    InsideGC = false;

                    BasicConsole.SetTextColour(BasicConsole.error_colour);
                    BasicConsole.WriteLine("Error! GC can't create a new string because the heap returned a null pointer.");
                    BasicConsole.DelayOutput(10);
                    BasicConsole.SetTextColour(BasicConsole.default_colour);

                    return null;
                }

                NumStrings++;

                //Initialise the GCHeader
                SetSignature(newObjPtr);
                //RefCount to 0 initially because of FOS_System.String.New should be used
                //      - In theory, New should be called, creates new string and passes it back to caller
                //        Caller is then required to store the string in a variable resulting in inc.
                //        ref count so ref count = 1 in only stored location. 
                //        Caller is not allowed to just "discard" (i.e. use Pop IL op or C# that generates
                //        Pop IL op) so ref count will always at some point be incremented and later
                //        decremented by managed code. OR the variable will stay in a static var until
                //        the OS exits...

                newObjPtr->RefCount = 0;

                FOS_System.String newStr = (FOS_System.String)Utilities.ObjectUtilities.GetObject(newObjPtr + 1);
                newStr._type = (FOS_System.Type)typeof(FOS_System.String);
                newStr.length = length;

                //Move past GCHeader
                byte* newObjBytePtr = (byte*)(newObjPtr + 1);

                InsideGC = false;

                return newObjBytePtr;
            }
            finally
            {
                ExitCritical();
            }
        }

        /// <summary>
        /// Increments the ref count of a GC managed object.
        /// </summary>
        /// <remarks>
        /// Uses underlying increment ref count method.
        /// </remarks>
        /// <param name="anObj">The object to increment the ref count of.</param>
        [Drivers.Compiler.Attributes.IncrementRefCountMethod]
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void IncrementRefCount(FOS_System.Object anObj)
        {
            if (!Enabled /*|| InsideGC*/ || anObj == null)
            {
                return;
            }

            InsideGC = true;

            byte* objPtr = (byte*)Utilities.ObjectUtilities.GetHandle(anObj);
            _IncrementRefCount(objPtr);

            InsideGC = false;
        }
        /// <summary>
        /// Underlying method that increments the ref count of a GC managed object.
        /// </summary>
        /// <remarks>
        /// This method checks that the pointer is not a null pointer and also checks for the GC signature 
        /// so string literals and the like don't accidentally get treated as normal GC managed strings.
        /// </remarks>
        /// <param name="objPtr">Pointer to the object to increment the ref count of.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void _IncrementRefCount(byte* objPtr)
        {
            if ((uint)objPtr < (uint)sizeof(GCHeader))
            {
                BasicConsole.SetTextColour(BasicConsole.error_colour);
                BasicConsole.WriteLine("Error! GC can't increment ref count of an object in low memory.");

                uint* basePtr = (uint*)ExceptionMethods.BasePointer;
                // Go up the linked-list of stack frames to (hopefully) the outermost caller
                basePtr = (uint*)*(basePtr);    // Frame of IncrementRefCount(x)
                uint retAddr = *(basePtr + 1);  // Caller of IncrementRefCount(x)
                basePtr = (uint*)*(basePtr);    // Frame of caller of IncrementRefCount(x)
                uint ret2Addr = *(basePtr + 1); // Caller of caller of IncrementRefCount(x)
                uint objAddr = (uint)objPtr;
                String msgStr = "Caller: 0x        , Object: 0x        , PCaller: 0x        ";
                // Object: 37
                // Caller: 17
                // PCaller: 58
                ExceptionMethods.FillString(retAddr, 17, msgStr);
                ExceptionMethods.FillString(objAddr, 37, msgStr);
                ExceptionMethods.FillString(ret2Addr, 58, msgStr);
                BasicConsole.WriteLine(msgStr);

                BasicConsole.DelayOutput(5);
                BasicConsole.SetTextColour(BasicConsole.default_colour);
            }
            
            objPtr -= sizeof(GCHeader);
            GCHeader* gcHeaderPtr = (GCHeader*)objPtr;
            if (CheckSignature(gcHeaderPtr))
            {
                if (gcHeaderPtr->CleanedUp)
                {
                    BasicConsole.WriteLine("Oops...Incrementing ref count of cleaned up object!");
                    BasicConsole.WriteLine(((FOS_System.Object)Utilities.ObjectUtilities.GetObject(gcHeaderPtr + 1))._Type.Signature);
                }

                gcHeaderPtr->RefCount++;

                if (gcHeaderPtr->RefCount > 0 && gcHeaderPtr->OnCleanupList)
                {
                    RemoveObjectToCleanup(gcHeaderPtr);
                }
            }
        }

        /// <summary>
        /// Decrements the ref count of a GC managed object.
        /// </summary>
        /// <remarks>
        /// This method checks that the pointer is not a null pointer and also checks for the GC signature 
        /// so string literals and the like don't accidentally get treated as normal GC managed strings.
        /// </remarks>
        /// <param name="anObj">The object to decrement the ref count of.</param>
        [Drivers.Compiler.Attributes.DecrementRefCountMethod]
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void DecrementRefCount(FOS_System.Object anObj)
        {
            DecrementRefCount(anObj, false);
        }
        /// <summary>
        /// Decrements the ref count of a GC managed object.
        /// </summary>
        /// <remarks>
        /// This method checks that the pointer is not a null pointer and also checks for the GC signature 
        /// so string literals and the like don't accidentally get treated as normal GC managed strings.
        /// </remarks>
        /// <param name="anObj">The object to decrement the ref count of.</param>
        /// <param name="overrideInside">Whether to ignore the InsideGC test or not.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void DecrementRefCount(FOS_System.Object anObj, bool overrideInside)
        {
            if (!Enabled /*|| (InsideGC && !overrideInside)*/ || anObj == null)
            {
                return;
            }

            if (!overrideInside)
            {
                InsideGC = true;
            }

            byte* objPtr = (byte*)Utilities.ObjectUtilities.GetHandle(anObj);
            _DecrementRefCount(objPtr);

            if (!overrideInside)
            {
                InsideGC = false;
            }
        }
        /// <summary>
        /// Underlying method that decrements the ref count of a GC managed object.
        /// </summary>
        /// <remarks>
        /// This method checks that the pointer is not a null pointer and also checks for the GC signature 
        /// so string literals and the like don't accidentally get treated as normal GC managed strings.
        /// </remarks>
        /// <param name="objPtr">A pointer to the object to decrement the ref count of.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void _DecrementRefCount(byte* objPtr)
        {
            if ((uint)objPtr < (uint)sizeof(GCHeader))
            {
                BasicConsole.SetTextColour(BasicConsole.error_colour);
                BasicConsole.WriteLine("Error! GC can't decrement ref count of an object in low memory.");

                uint* basePtr = (uint*)ExceptionMethods.BasePointer;
                // Go up the linked-list of stack frames to (hopefully) the outermost caller
                basePtr = (uint*)*(basePtr); // DecrementRefCount(x, y)
                basePtr = (uint*)*(basePtr); // DecrementRefCount(x)
                uint retAddr = *(basePtr + 1);
                uint objAddr = (uint)objPtr;
                String msgStr = "Caller: 0x        , Object: 0x        ";
                // Object: 37
                // Caller: 17
                ExceptionMethods.FillString(retAddr, 17, msgStr);
                ExceptionMethods.FillString(objAddr, 37, msgStr);
                BasicConsole.WriteLine(msgStr);

                BasicConsole.DelayOutput(5);
                BasicConsole.SetTextColour(BasicConsole.default_colour);
            }

            if (OutputTrace)
            {
                BasicConsole.WriteLine("GC-DP: 1");
            }
            GCHeader* gcHeaderPtr = (GCHeader*)(objPtr - sizeof(GCHeader));
            if (OutputTrace)
            {
                BasicConsole.WriteLine("GC-DP: 2");
            }
            if (CheckSignature(gcHeaderPtr))
            {
                if (OutputTrace)
                {
                    BasicConsole.WriteLine("GC-DP: 3");
                }

                gcHeaderPtr->RefCount--;

                //If the ref count goes below 0 then there was a circular reference somewhere.
                //  In actuality we don't care we can just only do cleanup when the ref count is
                //  exactly 0.
                if (gcHeaderPtr->RefCount == 0)
                {
#if GC_TRACE
                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine("Object ref count hit zero.");
                    }
#endif
                    FOS_System.Object obj = (FOS_System.Object)Utilities.ObjectUtilities.GetObject(objPtr);
                    if (obj is FOS_System.Array)
                    {
                        //Decrement ref count of elements
                        FOS_System.Array arr = (FOS_System.Array)obj;
                        if (!arr.elemType.IsValueType)
                        {
                            FOS_System.Object[] objArr = (FOS_System.Object[])Utilities.ObjectUtilities.GetObject(objPtr);
                            for (int i = 0; i < arr.length; i++)
                            {
                                DecrementRefCount(objArr[i], true);
                            }
                        }
                    }
                    //Cleanup fields
                    FieldInfo* FieldInfoPtr = obj._Type.FieldTablePtr;
                    //Loop through all fields. The if-block at the end handles moving to parent
                    //  fields. 
                    while (FieldInfoPtr != null)
                    {
                        if (FieldInfoPtr->Size > 0)
                        {
                            FOS_System.Type fieldType = (FOS_System.Type)Utilities.ObjectUtilities.GetObject(FieldInfoPtr->FieldType);
                            if (!fieldType.IsValueType &&
                                !fieldType.IsPointer)
                            {
                                byte* fieldPtr = objPtr + FieldInfoPtr->Offset;
                                FOS_System.Object theFieldObj = (FOS_System.Object)Utilities.ObjectUtilities.GetObject(fieldPtr);
                                
                                DecrementRefCount(theFieldObj, true);

#if GC_TRACE
                                if (OutputTrace)
                                {
                                    BasicConsole.WriteLine("Decremented ref count of field.");
                                }
#endif
                            }
                            
                            FieldInfoPtr++;
                        }

                        if (FieldInfoPtr->Size == 0)
                        {
                            FieldInfoPtr = (FieldInfo*)FieldInfoPtr->FieldType;
                        }
                    }

#if GC_TRACE
                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine("Adding object to cleanup list.");
                    }
#endif
                    AddObjectToCleanup(gcHeaderPtr, objPtr);
                }
            }
            if (OutputTrace)
            {
                BasicConsole.WriteLine("GC-DP: 4");
            }
        }

        /// <summary>
        /// Checks the GC header is valid by checking for the GC signature.
        /// </summary>
        /// <param name="headerPtr">A pointer to the header to check.</param>
        /// <returns>True if the signature is found and is correct.</returns>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static unsafe bool CheckSignature(GCHeader* headerPtr)
        {
            bool OK = headerPtr->Sig1 == 0x5C0EADE2U;
            OK = OK && headerPtr->Sig2 == 0x5C0EADE2U;
            OK = OK && headerPtr->Checksum == 0xB81D5BC4U;
            return OK;
        }
        /// <summary>
        /// Sets the GC signature in the specified GC header.
        /// </summary>
        /// <param name="headerPtr">A pointer to the header to set the signature in.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void SetSignature(GCHeader* headerPtr)
        {
            headerPtr->Sig1 = 0x5C0EADE2U;
            headerPtr->Sig2 = 0x5C0EADE2U;
            headerPtr->Checksum = 0xB81D5BC4U;
        }

        /// <summary>
        /// Scans the CleanupList to free objects from memory.
        /// </summary>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        public static void Cleanup()
        {
            if (!Enabled /*|| InsideGC*/)
            {
                return;
            }
            
            EnterCritical("Cleanup");

#if GC_TRACE
            int startNumObjs = NumObjs;
            int startNumStrings = NumStrings;
#endif

            try
            {
                InsideGC = true;

                if (OutputTrace)
                {
                    BasicConsole.WriteLine(" > Inside GC & Cleaning...");
                }

                ObjectToCleanup* currObjToCleanupPtr = CleanupList;
                ObjectToCleanup* prevObjToCleanupPtr = null;
                
                if (OutputTrace)
                {
                    BasicConsole.WriteLine(" > Got list...");
                }
                
                while (currObjToCleanupPtr != null)
                {
                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine(" > Item not null.");

                        FOS_System.String str1 = " > Item: 0x        ";
                        FOS_System.String str2 = " > Prev: 0x        ";
                        ExceptionMethods.FillString((uint)currObjToCleanupPtr, 18, str1);
                        ExceptionMethods.FillString((uint)currObjToCleanupPtr->prevPtr, 18, str2);
                        BasicConsole.WriteLine(str1);
                        BasicConsole.WriteLine(str2);
                    }

                    GCHeader* objHeaderPtr = currObjToCleanupPtr->objHeaderPtr;
                    objHeaderPtr->CleanedUp = true;

                    void* objPtr = currObjToCleanupPtr->objPtr;

                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine(" > Got object handles.");
                    }

                    if (objHeaderPtr->RefCount <= 0)
                    {
                        if (OutputTrace)
                        {
                            BasicConsole.WriteLine("   > Ref count zero or lower.");
                        }

                        FOS_System.Object obj = (FOS_System.Object)Utilities.ObjectUtilities.GetObject(objPtr);

                        if (OutputTrace)
                        {
                            BasicConsole.WriteLine("   > Got object.");
                        }

                        if (obj is FOS_System.String)
                        {
                            if (OutputTrace)
                            {
                                BasicConsole.WriteLine("   > (It's a string).");
                                BasicConsole.WriteLine((FOS_System.String)obj);
                            }

                            NumStrings--;
                        }
                        else
                        {
                            if (OutputTrace)
                            {
                                BasicConsole.WriteLine("   > (It's NOT a string).");
                            }

                            NumObjs--;
                        }

                        if (OutputTrace)
                        {
                            BasicConsole.WriteLine("   > About to free object...");
                        }

                        objHeaderPtr->OnCleanupList = false;
                        Heap.Free(objHeaderPtr);

                        if (OutputTrace)
                        {
                            BasicConsole.WriteLine("   > Object freed.");
                        }

                        if (OutputTrace)
                        {
                            BasicConsole.WriteLine("   > Done.");
                        }
                    }

                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine(" > Shifting to next item...");
                    }

                    prevObjToCleanupPtr = currObjToCleanupPtr;
                    currObjToCleanupPtr = currObjToCleanupPtr->prevPtr;

                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine(" > Removing object to cleanup...");
                    }
   
                    RemoveObjectToCleanup(prevObjToCleanupPtr);

                    if (OutputTrace)
                    {
                        BasicConsole.WriteLine(" > Done.");
                        BasicConsole.WriteLine(" > Loop back...");
                    }
                }

                InsideGC = false;
            }
            finally
            {
                ExitCritical();
            }

#if GC_TRACE
            if (OutputTrace)
            {
                PrintCleanupData(startNumObjs, startNumStrings);
            }
#endif
        }
        /// <summary>
        /// Outputs, via the basic console, how much memory was cleaned up.
        /// </summary>
        /// <param name="startNumObjs">The number of objects before the cleanup.</param>
        /// <param name="startNumStrings">The number of strings before the cleanup.</param>
        private static void PrintCleanupData(int startNumObjs, int startNumStrings)
        {
            int numObjsFreed = startNumObjs - NumObjs;
            int numStringsFreed = startNumStrings - NumStrings;
            BasicConsole.SetTextColour(BasicConsole.warning_colour);
            BasicConsole.WriteLine(((FOS_System.String)"Freed objects: ") + numObjsFreed);
            BasicConsole.WriteLine(((FOS_System.String)"Freed strings: ") + numStringsFreed);
            BasicConsole.WriteLine(((FOS_System.String)"Used memory  : ") + (Heap.FBlock->used * Heap.FBlock->bsize) + " / " + Heap.FBlock->size);
            BasicConsole.DelayOutput(2);
            BasicConsole.SetTextColour(BasicConsole.default_colour);
        }

        /// <summary>
        /// Adds an object to the cleanup list.
        /// </summary>
        /// <param name="objHeaderPtr">A pointer to the object's header.</param>
        /// <param name="objPtr">A pointer to the object.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        private static void AddObjectToCleanup(GCHeader* objHeaderPtr, void* objPtr)
        {
            EnterCritical("AddObjectToCleanup");

            try
            {
                objHeaderPtr->OnCleanupList = true;

                ObjectToCleanup* newObjToCleanupPtr = (ObjectToCleanup*)Heap.AllocZeroed((uint)sizeof(ObjectToCleanup), "GC : AddObjectToCleanup");
                newObjToCleanupPtr->objHeaderPtr = objHeaderPtr;
                newObjToCleanupPtr->objPtr = objPtr;

                if (CleanupList != null)
                {
                    newObjToCleanupPtr->nextPtr = null;
                    newObjToCleanupPtr->prevPtr = CleanupList;
                    CleanupList->nextPtr = newObjToCleanupPtr;
                }
                else
                {
                    newObjToCleanupPtr->prevPtr = null;
                    newObjToCleanupPtr->nextPtr = null;
                }
                
                CleanupList = newObjToCleanupPtr;
            }
            finally
            {
                ExitCritical();
            }
        }
        /// <summary>
        /// Removes an object from the cleanup list.
        /// </summary>
        /// <param name="objHeaderPtr">A pointer to the object's header.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        private static void RemoveObjectToCleanup(GCHeader* objHeaderPtr)
        {
            EnterCritical("RemoveObjectToCleanup");

            try
            {
                ObjectToCleanup* currObjToCleanupPtr = CleanupList;
                while (currObjToCleanupPtr != null)
                {
                    if (currObjToCleanupPtr->objHeaderPtr == objHeaderPtr)
                    {
                        objHeaderPtr->OnCleanupList = false;

                        RemoveObjectToCleanup(currObjToCleanupPtr);
                        return;
                    }
                    currObjToCleanupPtr = currObjToCleanupPtr->prevPtr;
                }
            }
            finally
            {
                ExitCritical();
            }
        }
        /// <summary>
        /// Removes an object from the cleanup list.
        /// </summary>
        /// <param name="objToCleanupPtr">A pointer to the cleanup-list element.</param>
        [Drivers.Compiler.Attributes.NoDebug]
        [Drivers.Compiler.Attributes.NoGC]
        private static void RemoveObjectToCleanup(ObjectToCleanup* objToCleanupPtr)
        {
            ObjectToCleanup* prevPtr = objToCleanupPtr->prevPtr;
            ObjectToCleanup* nextPtr = objToCleanupPtr->nextPtr;
            if (prevPtr != null)
            {
                prevPtr->nextPtr = nextPtr;
            }
            if (nextPtr != null)
            {
                nextPtr->prevPtr = prevPtr;
            }

            if(CleanupList == objToCleanupPtr)
            {
                if (prevPtr != null)
                {
                    CleanupList = prevPtr;
                }
                else
                {
                    CleanupList = nextPtr;
                }
            }
            
            Heap.Free(objToCleanupPtr);
        }
    }
    public unsafe class GCState : FOS_System.Object
    {
        /// <summary>
        /// Whether the GC is currently executing. Used to prevent the GC calling itself (or ending up in loops with
        /// called methods re-calling the GC!)
        /// </summary>
        public bool InsideGC = false;

        public bool OutputTrace = false;

        public FOS_System.String lastEnabler = "";
        public FOS_System.String lastDisabler = "";
        public FOS_System.String lastLocker = "[NEVER SET]";

        public SpinLock AccessLock = null;
        public bool AccessLockInitialised = false;

        /// <summary>
        /// The total number of objects currently allocated by the GC.
        /// </summary>
        public int NumObjs = 0;
        /// <summary>
        /// The number of strings currently allocated on the heap.
        /// </summary>
        public int NumStrings = 0;

        /// <summary>
        /// The linked-list of objects to clean up.
        /// </summary>
        public ObjectToCleanup* CleanupList = null;
    }
    
    /// <summary>
    /// Represents the GC header that is put in memory in front of every object so the GC can manage the object.
    /// </summary>
    public struct GCHeader
    {
        /// <summary>
        /// The first 4 bytes of the GC signature.
        /// </summary>
        public uint Sig1;
        /// <summary>
        /// The second 4 bytes of the GC signature.
        /// </summary>
        public uint Sig2;
        /// <summary>
        /// A checksum value.
        /// </summary>
        public UInt32 Checksum;

        /// <summary>
        /// The current reference count for the object associated with this header.
        /// </summary>
        public int RefCount;

        public bool OnCleanupList;
        public bool CleanedUp;
    }
    /// <summary>
    /// Represents an object to be garbage collected (i.e. freed from memory).
    /// </summary>
    public unsafe struct ObjectToCleanup
    {
        /// <summary>
        /// The pointer to the object.
        /// </summary>
        public void* objPtr;
        /// <summary>
        /// The pointer to the object's header.
        /// </summary>
        public GCHeader* objHeaderPtr;
        /// <summary>
        /// A pointer to the previous item in the cleanup list.
        /// </summary>
        public ObjectToCleanup* prevPtr;
        /// <summary>
        /// A pointer to the next item in the cleanup list.
        /// </summary>
        public ObjectToCleanup* nextPtr;
    }
}
