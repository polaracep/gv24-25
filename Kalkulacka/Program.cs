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

    public static bool ToBase(List<string> equation, int index)
    {
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 1), out fnValX)) return false;
        if (!Double.TryParse(equation.ElementAtOrDefault(index + 2), out fnValY)) return false;
        if (fnValY != 2 && fnValY != 8 && fnValY != 10 && fnValY != 16) return false;
        equation[index] = Convert.ToString((int)fnValX, (int)fnValY);
        return true;
    }
}

public class Menu
{
    private List<string> _command = new List<string>();
    private List<Variable> _variables = new List<Variable>();

    // neni nutno, neni nutno, aby byly tyhle veci List
    private string[] _availableOperators = new string[] { "*", "%", "/", "+", "-" };
    private string[] _availableFunctions = new string[] { "pow", "sqrt", "frombase", "tobase", "sin", "cos", "tan" };

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
                Console.WriteLine($"{v.Name}: {v.Value}");
            }
        }

        Console.Write("> ");
    }

    //using NewLineHelper;
    public void evalCommand(string cmdInput)
    {
        kalkulOutput = "";
        if (cmdInput == null)
            return;

        cmdInput = cmdInput.ToLower().Trim();
        _command = new List<string>(cmdInput.Split());

        // nejdriv evaluuj prikazy
        switch (_command.ElementAtOrDefault(0))
        {
            case "var":
                // help
                if (_command.ElementAtOrDefault(1) == null)
                {
                    kalkulOutput += "Usage:".Ln();
                    kalkulOutput += "\tnew <name> <val> \tcreate a new variable".Ln();
                    kalkulOutput += "\t<name>\t\t\tedit the value of a variable".Ln();
                    return;
                }
                // tvoř!
                if (_command.ElementAtOrDefault(1) == "new")
                {
                    string name;
                    double varValue;

                    // bereme argumenty! 2->jmeno
                    if (_command.ElementAtOrDefault(2) != null)
                    {
                        name = _command.ElementAtOrDefault(2)!;
                    }
                    // pojmenujem promennou
                    else
                    {
                        Console.WriteLine("Jak si svoji proměnnou pojmenuješ?");
                        // beru jen jmena bez mezer  
                        name = Console.ReadLine()!.Split(" ")[0];
                    }

                    // zjistujeme, zda uz jmeno existuje, ci je to nejaky rezervovany string
                    if (_variables.FindByName(name) != null ||
                        _availableOperators.Contains(name) || _availableFunctions.Contains(name))
                    {
                        kalkulOutput += "Promenna s takovym nazvem uz existuje!".Ln();
                        return;
                    }

                    // argument hodnota
                    if (_command.ElementAtOrDefault(3) != null)
                    {
                        if (Double.TryParse(_command.ElementAtOrDefault(3), out varValue))
                        {
                            _variables.Add(new Variable(name, Math.Round(varValue, precision)));
                            return;
                        }
                    }

                    // jinak zjisti hodnotu
                    Console.WriteLine("Hodnota promenne?");
                    while (!Double.TryParse(Console.ReadLine(), out varValue)) ;
                    _variables.Add(new Variable(name, Math.Round(varValue, precision)));

                    return;
                }
                // najit promennou, kterou budu upravovat
                if (_variables.FindByName(_command.ElementAtOrDefault(1)!) != null)
                {
                    double varValue;
                    Variable usedVar = _variables.FindByName(_command.ElementAtOrDefault(1)!);
                    // ctem input
                    while (!Double.TryParse(Console.ReadLine(), out varValue)) ;
                    usedVar.Value = varValue;
                    return;

                }
                kalkulOutput += ("To neni nazev promenne!").Ln(); ;
                return;

            case "precision":
                if (_command.ElementAtOrDefault(1) == null)
                {
                    kalkulOutput += "Usage:".Ln();
                    kalkulOutput += "\t<num> precision in decimal".Ln();
                    return;
                }
                break;

            case "exit":
                Environment.Exit(0);
                break;
            case "":
                kalkulOutput += "\"help\" pro seznam prikazu".Ln();
                return;
        }

        // pocitame!
        string result;
        if (Magic(_command, out result))
        {
            kalkulOutput += result;
        }
        else
        {
            if (kalkulOutput == "")
                kalkulOutput += "Spatny vyraz!";
            return;
        }
    }

    // pocitanicko
    public bool Magic(List<string> equation, out string stringOutput)
    {
        stringOutput = "";
        double output = 0;

        /*
         * 1) Jako prvni nahradime nazvy promennych jejich hodnotami, at s nimi muzeme pocitat
         * 2) nasledne evaluujeme funkce jako sqrt(), protoze nepodlehaji klasickemu "cislo operator cislo" syntaxu, ale muze nastat napr. "cislo operator fkce cislo" (3+sqrt(2))
         * -> evaluaci nahradime funkci vypoctenym cislem
         * 3) muzeme pocitat podle poradi operatoru
         */

        // transformuj promenne na cisla 
        for (int i = 0; i < equation.Count; i++)
        {
            Variable v = _variables.Find(x => x.Name == equation[i])!;
            if (v != null)
            {
                equation[i] = v.Value.ToString();
            }
        }

        // evaluuj funkce odzadu
        for (int i = equation.Count - 1; i >= 0; i--)
        // for (int i = 0; i < equation.Count; i++)
        {
            if (_availableFunctions.Contains(equation[i]))
            {
                switch (equation[i])
                {
                    // s tobase nemuzeme pouzivat nic jineho
                    case "tobase":
                        if (Compute.ToBase(equation, i) == false)
                            return false;
                        stringOutput = equation[i];
                        return true;
                    case "sin":
                        if (Compute.Sin(equation, i) == false)
                            return false;
                        break;
                    case "cos":
                        if (Compute.Sin(equation, i) == false)
                            return false;
                        break;
                    case "tan":
                        if (Compute.Sin(equation, i) == false)
                            return false;
                        break;
                    case "sqrt":
                        if (Compute.Sqrt(equation, i) == false)
                            return false;
                        break;
                    case "pow":
                        break;
                    case "frombase":
                        if (Compute.FromBase(equation, i, out bool baseError) == false)
                        {
                            if (baseError == true) kalkulOutput = "Spatna base".Ln();
                            return false;
                        }
                        break;
                }
            }
        }

        /* poradi: * > % > / > + > - 
         * algoritmus:
         * 1) najdi prvni nejvetsi operator
         * 2) cisla vedle nej zoperatoruj
         * 3) uloz cislo zpet do arraye na misto jednoho z tech cisel
         * 4) vymaz operator z arraye
         * 5) opakuj, dokud nevyzkousime vsechny operatory
         * -> v pripade, ze jsou vsechny vycerpany a velikost neni 1, je neco spatne
         */

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
                    double a, b;
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
                    stringOutput = Math.Round(a, precision).ToString();
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
            menu.evalCommand(Console.ReadLine() ?? "");
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

