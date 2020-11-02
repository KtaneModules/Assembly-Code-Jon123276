using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class AssemblyCode : MonoBehaviour
{
    public KMBombInfo bomb;
    public KMBombModule module;
    public KMAudio audio;
    public TextMesh[] textStuff;
    public KMSelectable[] buttons;
    private static int moduleId = 0;
    string[] code = "AAA AAD AAM AAS ADC ADD AND CALL CBW CLC CLD CLI CMC CMP CMPSB CMPSW CWD DAA DAS DEC DIV ESC HLT IDIV IMUL IN INC INT INTO IRET Jcc JCXZ JMP LAHF LDS LEA LES LOCK LODSB LODSW LOOP MOV MOVSB MOVSW MUL NEG NOP NOT OR OUT POP POPF PUSH PUSHF RCL RCR REPxx RET RETN RETF ROL ROR SAHF SAL SAR SBB SCASB SCASW SHL SHR STC STD STI STOSB STOSW SUB TEST WAIT XCHG XLAT XOR".Split(' ');
    string[] actual = "ASCII adjust AL after addition,ASCII adjust AX before division,ASCII adjust AX after multiplication,ASCII adjust AL after subtraction,Add with carry,Add,Logical AND,Call procedure,Convert byte to word,Clear carry flag,Clear direction flag,Clear interrupt flag,Complement carry flag,Compare operands,Compare bytes in memory,Compare words,Convert word to doubleword,Decimal adjust AL after addition,Decimal adjust AL after subtraction,Decrement by 1,Unsigned divide,Used with floating-point unit,Enter halt state,Signed divide,Signed multiply,Input from port,Increment by 1,Call to interrupt,Call to interrupt if overflow,Return from interrupt,Jump if condition,Jump if CX is zero,Jump,Load FLAGS into AH register,Load pointer using DS,Load Effective Address,Load ES with pointer,Assert BUS LOCK# signal,Load string byte,Load string word,Loop control,Move,Move byte from string to string,Move word from string to string,Unsigned multiply,Two's complement negation,No operation,Negate the operand logical NOT,Logical OR,Output to port,Pop data from stack,Pop FLAGS register from stack,Push data onto stack,Push FLAGS onto stack,Rotate left (with carry),Rotate right (with carry),Repeat MOVS/STOS/CMPS/LODS/SCAS,Return from procedure,Return from near procedure,Return from far procedure,Rotate left,Rotate right,Store AH into FLAGS,Shift Arithmetically left (signed shift left),Shift Arithmetically right (signed shift right),Subtraction with borrow,Compare byte string,Compare word string,Shift left (unsigned shift left),Shift right (unsigned shift right),Set carry flag,Set direction flag,Set interrupt flag,Store byte in string,Store word in string,Subtraction,Logical compare (AND),Wait until not busy,Exchange data,Table look-up translation,Exclusive OR".Split(',');
    int a = 0;
    int stage = 1;
    string assembly = "";
    int selecting = 0;
    bool solved = false;
    struct codeToActual
    {
        public string cod;
        public string act;
    }

    List<codeToActual> ant = new List<codeToActual>()
    {
        
    };
    void GenerateStuff()
    {
        for (int i = 0; i < 8; i++)
        {
            assembly += assembler();
            assembly += "\n";
        }
        textStuff[0].text += assembly;
        textStuff[1].text = actual[a];
        codeToSelect();
    }
    void Start()
    {
        moduleId++;
        GenerateStuff();
        textStuff[2].text = a.ToString();
        buttons[0].OnInteract += delegate
        {
            Left();
            return false;
        };
        buttons[1].OnInteract += delegate
        {
            Right();
            return false;
        };
        buttons[2].OnInteract += delegate
        {
            Submitting();
            return false;
        };
    }
    string assembler()
    {
        int c = Rnd.Range(0, code.Length);
        string a = code[c];
        string b = actual[c];
        Debug.LogFormat("[Assembly Code #{0}] Your code is {1}, {2}, and the definition is {3}, {4}", moduleId, a, c, b, c);
        ant.Add(new codeToActual { cod = a, act = b });
        return a;
    }
    void Left()
    {
        a--;
        if (a < 0)
        {
            a = 0;
        }
        textStuff[1].text = actual[a];
        textStuff[2].text = a.ToString();
    }
    void Right()
    {
        a++;
        if (a > actual.Length-1)
        {
            a = actual.Length-1;
        }
        textStuff[1].text = actual[a];
        textStuff[2].text = a.ToString();
    }
    void Submitting()
    {
        if (ant[selecting].act.Equals(textStuff[1].text))
        {
            if (stage == 3)
            {
                solved = true;
                assembly = "";
                textStuff[2].text = "✓";
                textStuff[1].text = "Good job!";
                textStuff[0].text = "";
                module.HandlePass();
            }
            else
            {
                stage++;
                ant.Clear();
                assembly = "";
                selecting = 0;
                textStuff[0].text = "";
                GenerateStuff();
            }
        }
        else
        {
            module.HandleStrike();
        }
    }
    void codeToSelect()
    {
        if ((bomb.GetModuleIDs().Where(x => x == "brainf").Count() > 0) || (bomb.GetModuleIDs().Where(x => x == "KritScripts").Count() > 0) || (bomb.GetModuleIDs().Where(x => x == "linesOfCode").Count() > 0))
        {
            selecting = 0;
        }
        else
        {
            selecting += (2 * bomb.GetBatteryCount(Battery.AA)) + 1;
            selecting %= 8;
            selecting++;
        }
        Debug.LogFormat("[Assembly Code #{0}] The display should say: {1}", moduleId, ant[selecting].act);
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} L/R # [Presses the left/right button # number of times.] | Valid numbers are 1-9. | !{0} submit [Submits the current definition";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        yield return null;
        if (Regex.IsMatch(parameters[0], @"^\s*L\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length < 2) { yield return "sendtochaterror Please specify how many times you want this button to be pressed (from 1 to 80.)"; yield break; }
            for (int i = 0; i < int.Parse(parameters[1])%81; i++)
            {
                buttons[0].OnInteract();
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (Regex.IsMatch(parameters[0], @"^\s*R\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length < 2) { yield return "sendtochaterror Please specify how many times you want this button to be pressed (from 1 to 80.)"; yield break; }
            for (int i = 0; i < int.Parse(parameters[1])%81; i++)
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[2].OnInteract();
        }
        else
        {
            yield return "sendtochaterror Please specify want you want to press and how many times of such button.";
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        int start = stage;
        for (int i = start; i < 4; i++)
        {
            if (a < Array.IndexOf(actual, ant[selecting].act))
            {
                while (a < Array.IndexOf(actual, ant[selecting].act))
                {
                    buttons[1].OnInteract();
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else if (a > Array.IndexOf(actual, ant[selecting].act))
            {
                while (a > Array.IndexOf(actual, ant[selecting].act))
                {
                    buttons[0].OnInteract();
                    yield return new WaitForSeconds(0.01f);
                }
            }
            buttons[2].OnInteract();
            yield return new WaitForSeconds(0.01f);
        }
    }
}

