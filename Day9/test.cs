using System;
using System.Collections.Generic;
using System.Text;

namespace Day9
{

    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.Linq;

    namespace IntCodeMachine
    {
        public class ICMachine
        {

            #region Variables

            long[] State;
            long PC;
            bool Abort;
            Dictionary<long, Action> Opcodes;
            Action[] Operands;
            long OpParams;
            long RB;

            #endregion

            #region Multithreading Support

            AutoResetEvent InputEvent = new AutoResetEvent(false);
            AutoResetEvent OutputEvent = new AutoResetEvent(false);
            AutoResetEvent AbortEvent = new AutoResetEvent(false);

            #endregion

            #region Properties

            public Queue<long> Output { get; } = new Queue<long>();
            public Queue<long> Input { get; } = new Queue<long>();

            public bool Trace { get; set; } = true;

            public bool InteractiveMode { get; set; } = true;
            public bool Running { get; private set; } = false;

            #endregion

            #region Public I/O Handling

            public void AwaitOutput()
            {
                OutputEvent.WaitOne();
            }

            public void ProvideInput(int Value)
            {
                Input.Enqueue(Value);
                InputEvent.Set();
            }

            #endregion

            #region Configuration & Operation

            public static long[] ParseFile(string Filename = "input.txt") => System.IO.File.ReadAllText(Filename).Split(',').Select(x => long.Parse(x)).ToArray();

            public void SetVerb(long Verb)
            {
                writeAddress(2, Verb);
            }

            public void SetNoun(long Noun)
            {
                writeAddress(1, Noun);
            }

            public void LoadState(long[] NewState)
            {
                Halt();
                while (Running)
                    Thread.Sleep(10);

                State = new long[NewState.Length];
                Array.Copy(NewState, 0, State, 0, NewState.Length);
                Input.Clear();
                Output.Clear();
                RB = 0;
                AbortEvent.Reset();
                InputEvent.Reset();
                OutputEvent.Reset();
            }

            public void ExecuteThreaded(long ProgramCounter = 0, string ThreadName = "VM Thread")
            {
                Thread Async = new Thread((x) => Execute((int)x));
                Async.Name = ThreadName;
                Async.Start(ProgramCounter);
            }

            public void Execute(long ProgramCounter = 0)
            {
                if (Running)
                    throw new InvalidOperationException("Cannot start execution on non-halted VM");

                PC = ProgramCounter;
                Abort = false;
                Running = true;

                while (!Abort)
                {
                    if (Trace)
                        Console.Write("{0:X4}: ", PC);
                    long Instruction = (OpParams = instructionRead()) % 100;
                    OpParams /= 100;
                    Operands[Instruction]();
                    if (Trace)
                        Console.WriteLine();
                }

                Running = false;
            }

            public void Resume() => Execute(PC);

            public void Halt()
            {
                Abort = true;
                AbortEvent.Set();
            }

            #endregion

            #region Construction & Operands

            public ICMachine(string Filename) : this()
            {
                LoadState(ParseFile(Filename));
            }

            public ICMachine()
            {
                Opcodes = new Dictionary<long, Action> {
                // 1 - Add
                {1,  () => {long Addr1 = instructionRead(); long Addr2 = instructionRead(); writeParameter(instructionRead(), readParameter(Addr1) + readParameter(Addr2));}},

                //2 - Multiply
                {2,  () => {long Addr1 = instructionRead(); long Addr2 = instructionRead(); writeParameter(instructionRead(), readParameter(Addr1) * readParameter(Addr2));}},

                // 3 - Write input to memory
                {3, () => { GetInput(); } },

                // 4 - Diagnostic output
                {4, () => { if(InteractiveMode) { if (Trace) Console.Write(" > Output: {0}", readParameter()); else Console.WriteLine("Output: {0}", readParameter()); } else { Output.Enqueue(readParameter()); OutputEvent.Set(); } } },

                // 5 - Jump if true
                {5, () => { if (readParameter() != 0) PC = readParameter(); else PC++; } },

                // 6 - Jump if false
                {6, () => { if (readParameter() == 0) PC = readParameter(); else PC++; } },
                
                // 7 - Less Than
                {7, () => { long Param1 = readParameter(); long Param2 = readParameter(); writeParameter(instructionRead(), (Param1 < Param2) ? 1 : 0); } },
                
                // 8 - Equals
                {8, () => { long Param1 = readParameter(); long Param2 = readParameter(); writeParameter(instructionRead(), (Param1 == Param2) ? 1 : 0); } },

                // 9 - Relative Base
                {9, () => { RB += readParameter(); } },

                // 99 - Halt
                {99,  () => {Abort = true;}},
            };

                // Convert from sparse dictionary to flat array
                Operands = new Action[Opcodes.Keys.Max() + 1];
                Action InvalidOpcode = () => { PC--; Console.WriteLine("Invalid opcode {0} at {1}", readAddress(PC), PC); Abort = true; };

                for (int i = 0; i < Operands.Length; i++)
                    Operands[i] = InvalidOpcode;

                foreach (var Opcode in Opcodes)
                    Operands[Opcode.Key] = Opcode.Value;
            }

            #endregion

            #region Internal Support Functions

            private void GetInput()
            {
                if (Input.Count == 0 && InteractiveMode)
                {
                    Console.Write("Input Requested: ");
                    writeParameter(instructionRead(), long.Parse(Console.ReadLine()));
                }
                else
                {
                    int Which = 0;
                    if (Input.Count == 0)
                        Which = WaitHandle.WaitAny(new WaitHandle[] { InputEvent, AbortEvent });
                    else
                        InputEvent.Reset();

                    if (Which == 1)
                        return;

                    writeParameter(instructionRead(), Input.Dequeue());
                }
            }

            internal long instructionRead()
            {
                if (Trace)
                    Console.Write(" {0}", State[PC]);
                return State[PC++];
            }

            private long readAddress(long Address)
            {
                if (Address < 0)
                    Address = 0;
                if (Address >= State.Length)
                    Array.Resize(ref State, (int)Address + 1);

                return State[Address];
            }

            private long readParameter() => readParameter(instructionRead());

            private long readParameter(long Param)
            {
                long ParamMode = OpParams % 10;
                OpParams /= 10;

                switch (ParamMode)
                {
                    case 0:
                        return readAddress(Param);
                    case 1:
                        return Param;
                    case 2:
                        return readAddress(Param + RB);
                    default:
                        return Param;
                }
            }

            private void writeParameter(long Param, long Value)
            {
                long ParamMode = OpParams % 10;
                OpParams /= 10;

                switch (ParamMode)
                {
                    case 0:
                        writeAddress(Param, Value);
                        break;
                    case 2:
                        writeAddress(Param + RB, Value);
                        break;
                }
            }

            private void writeAddress(long Address, long Value)
            {
                if (Address < 0)
                    Address = 0;
                if (Address >= State.Length)
                    Array.Resize(ref State, (int)Address + 1);

                State[Address] = Value;
            }

            #endregion

        }
    }
}
