namespace Kalkulacka;

public static class Helpers
{
    public static string Ln(this string kalkOut)
    {
        return kalkOut += Environment.NewLine;
    }

    // possible null return
    public static Variable FindByName(this List<Variable> _vars, string name)
    {
        return _vars.Find(a => a.Name.Contains(name))!;
    }
}

public class Variable
{
    string _name;
    double _value;

    public string Name { get => _name; set => _name = value; }
    public double Value { get => _value; set => _value = value; }

    public Variable(string name, double val)
    {
        _name = name;
        _value = val;
    }

    public Variable(string name)
    {
        _name = name;
        _value = 0;
    }

}

// fujky fujky fujky
public static class Compute
{

    static double fnValX;
    static double fnValY;

    public static bool Sin(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Sin(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

    static public bool Cos(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Cos(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

    static public bool Tan(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Tan(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

    public static bool Sqrt(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Sqrt(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

    public static bool Pow(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 2), out fnValY)) return false;
        equation[index] = Math.Pow(fnValX, fnValY).ToString();
        equation.RemoveAt(index + 2);
        equation.RemoveAt(index + 1);
        return true;
    }

    public static bool FromBase(List<string> equation, int index, out bool baseError)
    {
        baseError = false;
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 2), out fnValY)) return false;
        if (fnValY != 2 && fnValY != 8 && fnValY != 10 && fnValY != 16) return false;

        // kdyz budu davat pismenka do arg1 a base bude jina nez 16, hodi to error
        try { equation[index] = Convert.ToInt32(equation[index + 1], (int)fnValY).ToString(); }
        catch (Exception e)
        {
            // at mi to nerve
            e.ToString();
            baseError = true;
            return false;
        }
        equation.RemoveAt(index + 2);
        equation.RemoveAt(index + 1);
        return true;
    }

    public static bool ToBase(List<string> command)
    {
        if (!Double.TryParse(command.ElementAtOrDefault(1), out fnValX)) return false;
        if (!Double.TryParse(command.ElementAtOrDefault(2), out fnValY)) return false;
        if (fnValY != 2 && fnValY != 8 && fnValY != 10 && fnValY != 16) return false;
        command[0] = Convert.ToString((int)fnValX, (int)fnValY);
        return true;
    }

    public static bool Log(List<string> equation, int index)
    {
        // X = base, Y = value
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 2), out fnValY)) return false;
        //if (fnValX <= 0) return false;
        equation[index] = Math.Log(fnValY, fnValX).ToString();
        equation.RemoveAt(index + 2);
        equation.RemoveAt(index + 1);
        return true;
        // Math.Log(
    }

    public static bool Floor(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Floor(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

    public static bool Ceil(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        equation[index] = Math.Ceiling(fnValX).ToString();
        equation.RemoveAt(index + 1);
        return true;
    }

}

public class Menu
{
    private List<string> _command = new List<string>();
    private List<Variable> _variables = new List<Variable>();

    // neni nutno, neni nutno, aby byly tyhle veci List
    private string[] _availableOperators = new string[] { "*", "%", "/", "+", "-" };
    private string[] _availableFunctions = new string[] { "pow", "sqrt", "frombase", "tobase", "sin", "cos", "tan", "log", "floor", "ceil", "var", "precision", "exit" };

    public List<string> Command { get => _command; set => _command = value; }
    public List<Variable> Variables { get => _variables; set => _variables = value; }

    private string kalkulOutput = new String("");
    private int precision = 5;


    public void render()
    {
        // diky, chatgpt!
        Console.WriteLine(String.Concat(Enumerable.Repeat(Environment.NewLine, Console.WindowHeight)));

        Console.WriteLine($"= {kalkulOutput}");
        kalkulOutput = "";

        Console.WriteLine(new string('=', Console.WindowWidth));

        if (_variables.Count != 0)
        {
            Console.WriteLine("Promenne:");
            foreach (Variable v in _variables)
            {
                Console.WriteLine($"{v.Name}: {Math.Round(v.Value, precision)}");
            }
        }

        Console.Write("> ");
    }

    // Entry point pro kalkulacku
    public void inputHandler(string cmdInput)
    {
        kalkulOutput = "";
        if (cmdInput == null)
            return;

        cmdInput = cmdInput.ToLower().Trim();
        _command = new List<string>(cmdInput.Split());

        if (evalCommand() == true)
            return;

        // pocitame!
        if (computer(_command))
        {
            kalkulOutput += _command[0];
            return;
        }
        else
        {
            if (kalkulOutput == "")
                kalkulOutput += "Spatny vyraz!";
            return;
        }
    }

    private bool evalCommand()
    {
        // evaluuj prikazy
        switch (_command.ElementAtOrDefault(0))
        {
            case "":
                kalkulOutput += "\"help\" pro seznam prikazu".Ln();
                return false;
            case "help":
                kalkulOutput += "Mezi každým tokenem musí být mezera, např: \"1 + 2\",".Ln();
                kalkulOutput += "či: \"sqrt 2\"".Ln();
                kalkulOutput += "".Ln();
                kalkulOutput += "Dostupné funkce: ".Ln();
                foreach (string fce in _availableFunctions)
                {
                    kalkulOutput += "\t" + fce.Ln();
                }

                kalkulOutput += "Dostupné operatory: ".Ln();
                foreach (string fce in _availableOperators)
                {
                    kalkulOutput += "\t" + fce.Ln();
                }

                return true;
            case "var":
                // help
                if (_command.ElementAtOrDefault(1) == null)
                {
                    kalkulOutput += "Usage: ".Ln();
                    kalkulOutput += "\tvar <name> <value>".Ln();
                    return false;
                }

                // najit promennou, kterou budu upravovat
                // jestli uz neexistuje promenna, udelej novou
                if (_variables.FindByName(_command.ElementAtOrDefault(1)!) == null)
                {
                    string name = "";
                    double varValue;

                    // precti argumenty! 1->jmeno
                    if (_command.ElementAtOrDefault(1) != null)
                    {
                        name = _command.ElementAtOrDefault(1)!;
                    }

                    // zjistujeme, zda uz jmeno existuje, ci je to nejaky rezervovany string
                    if (_availableOperators.Contains(name) || _availableFunctions.Contains(name))
                    {
                        kalkulOutput += "skibidi error".Ln();
                        return false;
                    }

                    // argument hodnota
                    if (_command.ElementAtOrDefault(2) != null)
                    {
                        if (Double.TryParse(_command.ElementAtOrDefault(2), out varValue))
                        {
                            _variables.Add(new Variable(name, Math.Round(varValue, precision)));
                            return false;
                        }
                    }

                    // jinak zjisti hodnotu
                    Console.WriteLine("Hodnota promenne?");
                    while (!Double.TryParse(Console.ReadLine(), out varValue)) ;
                    _variables.Add(new Variable(name, Math.Round(varValue, precision)));
                }
                else
                {
                    double varValue;
                    Variable usedVar = _variables.FindByName(_command.ElementAtOrDefault(1)!);

                    // zkus parsnout argument, jestli to nevyjde, precti input uzivatele
                    if (!Double.TryParse(_command.ElementAtOrDefault(2), out varValue))
                    {
                        // ctem input
                        Console.WriteLine("Hodnota promenne?");
                        while (!Double.TryParse(Console.ReadLine(), out varValue)) ;
                    }
                    usedVar.Value = varValue;
                }
                return true;

            case "precision":
                if (_command.ElementAtOrDefault(1) == null)
                {
                    kalkulOutput += "Usage:".Ln();
                    kalkulOutput += "precision\t<num> of precision places".Ln();
                    return false;
                }
                else
                {
                    if (!Int32.TryParse(_command.ElementAtOrDefault(1), out int _prec))
                    {
                        kalkulOutput += "neni cislo :(".Ln();
                    }
                    if (_prec > 12)
                    {
                        kalkulOutput += "big number! no!".Ln();
                        return false;
                    }
                    precision = _prec;
                    kalkulOutput += $"Precision set to {precision} places".Ln();
                    return true;
                }
            case "tobase":
                if (Compute.ToBase(_command) == false)
                {
                    kalkulOutput += "Usage:".Ln();
                    kalkulOutput += "tobase\t<number> <base>".Ln();
                    return false;
                }
                kalkulOutput = _command[0];
                return true;
            case "exit":
                Environment.Exit(0);
                break;
        }
        return false;
    }

    private bool computer(List<string> equation)
    {
        bool completed;

        /*
         * 1) Jako prvni nahradime nazvy promennych jejich hodnotami, at s nimi muzeme pocitat
         * 2) nasledne evaluujeme funkce jako sqrt(), protoze nepodlehaji klasickemu "cislo operator cislo" syntaxu, ale muze nastat napr. "cislo operator fkce cislo" (3+sqrt(2))
         * -> evaluaci nahradime funkci vypoctenym cislem
         * 3) muzeme pocitat podle poradi operatoru
         */

        // transformuj promenne na cisla 
        evalVariables(equation);

        // evaluuj funkce (zezadu)
        evalFunctions(equation, out completed);
        if (completed == false) return false;

        /* poradi: * > % > / > + > - 
         * algoritmus:
         * 1) najdi prvni nejvetsi operator
         * 2) cisla vedle nej zoperatoruj
         * 3) uloz cislo zpet do arraye na misto jednoho z tech cisel
         * 4) vymaz operator z arraye
         * 5) opakuj, dokud nevyzkousime vsechny operatory
         * -> v pripade, ze jsou vsechny vycerpany a velikost neni 1, je neco spatne
         */
        return evalOperations(equation);
    }

    private void evalFunctions(List<string> equation, out bool returnValue)
    {
        returnValue = true;
        for (int i = equation.Count - 1; i >= 0; i--)
        // for (int i = 0; i < equation.Count; i++)
        {
            if (_availableFunctions.Contains(equation[i]))
            {
                switch (equation[i])
                {
                    // s tobase nemuzeme pouzivat nic jineho
                    case "sin":
                        if (Compute.Sin(equation, i) == false)
                            returnValue = false;
                        break;
                    case "cos":
                        if (Compute.Sin(equation, i) == false)
                            returnValue = false;
                        break;
                    case "tan":
                        if (Compute.Sin(equation, i) == false)
                            returnValue = false;
                        break;
                    case "sqrt":
                        if (Compute.Sqrt(equation, i) == false)
                            returnValue = false;
                        break;
                    case "pow":
                        if (Compute.Pow(equation, i) == false)
                            returnValue = false;
                        break;
                    case "frombase":
                        if (Compute.FromBase(equation, i, out bool baseError) == false)
                        {
                            kalkulOutput = "usage: <num> <base>".Ln();
                            if (baseError == true) kalkulOutput = "Spatna base".Ln();
                            returnValue = false;
                        }
                        break;
                    case "log":
                        if (Compute.Log(equation, i) == false)
                            returnValue = false;
                        break;
                    case "floor":
                        if (Compute.Floor(equation, i) == false)
                            returnValue = false;
                        break;
                    case "ceil":
                        if (Compute.Ceil(equation, i) == false)
                            returnValue = false;
                        break;
                }
            }
        }
    }

    private void evalVariables(List<string> equation)
    {
        for (int i = 0; i < equation.Count; i++)
        {
            Variable v = _variables.Find(x => x.Name == equation[i])!;
            if (v != null)
            {
                equation[i] = v.Value.ToString();
            }
        }
    }

    private bool evalOperations(List<string> equation)
    {
        // vsechny operatory postupne
        foreach (string operatorSearched in _availableOperators)
        {
            for (int i = 0; i < equation.Count(); i++)
            {
                // element je momentalni character ktery skenuju
                string element = equation.ElementAtOrDefault(i) ?? "";

                if (operatorSearched == element)
                {
                    // nasli jsme operator, sezeneme cisla vedle a provedeme vypocet
                    double a, b, output;
                    if (!Double.TryParse(equation.ElementAtOrDefault(i - 1), out a))
                        return false;
                    if (!Double.TryParse(equation.ElementAtOrDefault(i + 1), out b))
                        return false;

                    switch (equation.ElementAtOrDefault(i))
                    {
                        case "+":
                            output = a + b;
                            break;
                        case "-":
                            output = a - b;
                            break;
                        case "*":
                            output = a * b;
                            break;
                        case "/":
                            output = a / b;
                            break;
                        default:
                            return false;
                    }

                    equation[i] = output.ToString();
                    equation.RemoveAt(i + 1);
                    equation.RemoveAt(i - 1);
                    i -= 2;
                }
                if (equation.Count == 1)
                {
                    // nemame cislicko :(
                    if (!Double.TryParse(equation[0], out double a))
                    {
                        kalkulOutput += "\"help\" pro seznam prikazu";
                        return false;
                    }
                    equation[0] = Math.Round(a, precision).ToString();
                    return true;
                }
            }
        }
        return false;
    }
}


class Program
{
    static void Main(string[] args)
    {
        Menu menu = new Menu();
        menu.Variables.Add(new Variable("pi", Math.PI));

        // Loop
        while (true)
        {
            menu.render();
            menu.inputHandler(Console.ReadLine() ?? "");
        }

    }
}

/*
 * ZADANI
 * Vytvor program ktery bude fungovat jako kalkulacka. Kroky programu budou nasledujici:
 * 1) Nacte vstup pro prvni cislo od uzivatele (vyuzijte metodu Console.ReadLine() - https://learn.microsoft.com/en-us/dotnet/api/system.console.readline?view=netframework-4.8.
 * 2) Zkonvertuje vstup od uzivatele ze stringu do integeru - https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-convert-a-string-to-a-number.
 * 3) Nacte vstup pro druhe cislo od uzivatele a zkonvertuje ho do integeru. (zopakovani kroku 1 a 2 pro druhe cislo)
 * 4) Nacte vstup pro ciselnou operaci. Rozmysli si, jak operace nazves. Muze to byt "soucet", "rozdil" atd. nebo napr "+", "-", nebo jakkoliv jinak.
 * 5) Nadefinuj integerovou promennou result a prirad ji prozatimne hodnotu 0.
 * 6) Vytvor podminky (if statement), podle kterych urcis, co se bude s cisly dit podle zadane operace
 *    a proved danou operaci - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/selection-statements.
 * 7) Vypis promennou result do konzole
 * 
 * Rozsireni programu pro rychliky / na doma (na poradi nezalezi):
 * 1) Vypis do konzole pred nactenim kazdeho uzivatelova vstupu co po nem chces (aby vedel, co ma zadat)
 * 2) a) Kontroluj, ze uzivatel do vstupu zadal, co mel (cisla, popr. ciselnou operaci). Pokud zadal neco jineho, napis mu, co ma priste zadat a program ukoncete.
 * 2) b) To same, co a) ale misto ukonceni programu opakovane cti vstup, dokud uzivatel nezada to, co ma
 *       - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-while-statement
 * 3) Umozni uzivateli zadavat i desetinna cisla, tedy prekopej kalkulacku tak, aby umela pracovat s floaty
 */

