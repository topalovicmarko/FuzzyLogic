using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AI.Fuzzy.Library;

namespace FuzzyLogic
{
    public partial class Form1 : Form
    {
        SugenoFuzzySystem fsRobot;
        FuzzyVariable fvUdaljenost;
        FuzzyVariable fvUgao;
        SugenoVariable svSmerKretanja;

        public Form1()
        {
            InitializeComponent();
            NapraviSugenoSistem();

            double rez1 = IzracunajSugeno(40, 20);
            double rez2 = IzracunajSugeno(120, -1);

            lbl1.Text = $"a) Smer kretanja za udaljenost 40 cm i ugao 20 stepeni je: {rez1}";
            lbl2.Text = $"b) Smer kretanja za udaljenost 120 cm i ugao -1 stepen je: {rez2}";
        }

        public void NapraviSugenoSistem()
        {
            fsRobot = new SugenoFuzzySystem();

            fvUdaljenost = new FuzzyVariable("Udaljenost", 0, 500);
            fvUdaljenost.Terms.Add(new FuzzyTerm("blizu", new TriangularMembershipFunction(0, 10, 50)));
            fvUdaljenost.Terms.Add(new FuzzyTerm("daleko", new TriangularMembershipFunction(25, 150, 200)));
            fvUdaljenost.Terms.Add(new FuzzyTerm("veomaDaleko", new TriangularMembershipFunction(100, 250, 500)));
            fsRobot.Input.Add(fvUdaljenost);

            fvUgao = new FuzzyVariable("Ugao", -90, 90);
            fvUgao.Terms.Add(new FuzzyTerm("pozitivnoVeliki", new TriangularMembershipFunction(15, 45, 90)));
            fvUgao.Terms.Add(new FuzzyTerm("pozitivnoMali", new TriangularMembershipFunction(1, 20, 30)));
            fvUgao.Terms.Add(new FuzzyTerm("nula", new TriangularMembershipFunction(-5, 0, 5)));
            fvUgao.Terms.Add(new FuzzyTerm("negativnoMali", new TriangularMembershipFunction(-30, -20, -10)));
            fvUgao.Terms.Add(new FuzzyTerm("negativnoVeliki", new TriangularMembershipFunction(-90, -45, -15)));
            fsRobot.Input.Add(fvUgao);

            svSmerKretanja = new SugenoVariable("SmerKretanja");
            svSmerKretanja.Functions.Add(fsRobot.CreateSugenoFunction("pravo", new double[] { 0, 0, 0 }));
            svSmerKretanja.Functions.Add(fsRobot.CreateSugenoFunction("blagoLevo", new double[] { 0, 0, -20 }));
            svSmerKretanja.Functions.Add(fsRobot.CreateSugenoFunction("blagoDesno", new double[] { 0, 0, 20 }));
            svSmerKretanja.Functions.Add(fsRobot.CreateSugenoFunction("ostroLevo", new double[] { 0, 0, -45 }));
            svSmerKretanja.Functions.Add(fsRobot.CreateSugenoFunction("ostroDesno", new double[] { 0, 0, 45 }));
            fsRobot.Output.Add(svSmerKretanja);

            try
            {
                SugenoFuzzyRule pravilo1 = fsRobot.ParseRule("if (Udaljenost is blizu) and (Ugao is pozitivnoVeliki) then (SmerKretanja is ostroLevo)");
                SugenoFuzzyRule pravilo2 = fsRobot.ParseRule("if (Udaljenost is blizu) and (Ugao is negativnoVeliki) then (SmerKretanja is ostroDesno)");
                SugenoFuzzyRule pravilo3 = fsRobot.ParseRule("if (Udaljenost is blizu) and (Ugao is negativnoMali) then (SmerKretanja is blagoDesno)");
                SugenoFuzzyRule pravilo4 = fsRobot.ParseRule("if (Udaljenost is blizu) and (Ugao is pozitivnoMali) then (SmerKretanja is blagoLevo)");
                SugenoFuzzyRule pravilo5 = fsRobot.ParseRule("if (Udaljenost is daleko) and (Ugao is negativnoVeliki) then (SmerKretanja is ostroDesno)");
                SugenoFuzzyRule pravilo6 = fsRobot.ParseRule("if (Udaljenost is daleko) and (Ugao is pozitivnoVeliki) then (SmerKretanja is ostroLevo)");
                SugenoFuzzyRule pravilo7 = fsRobot.ParseRule("if (Udaljenost is daleko) and (Ugao is pozitivnoMali) then (SmerKretanja is blagoLevo)");
                SugenoFuzzyRule pravilo8 = fsRobot.ParseRule("if (Udaljenost is daleko) and (Ugao is negativnoMali) then (SmerKretanja is blagoDesno)");
                SugenoFuzzyRule pravilo9 = fsRobot.ParseRule("if (Udaljenost is veomaDaleko) and (Ugao is nula) then (SmerKretanja is pravo)");
                
                fsRobot.Rules.Add(pravilo1);
                fsRobot.Rules.Add(pravilo2);
                fsRobot.Rules.Add(pravilo3);
                fsRobot.Rules.Add(pravilo4);
                fsRobot.Rules.Add(pravilo5);
                fsRobot.Rules.Add(pravilo6);
                fsRobot.Rules.Add(pravilo7);
                fsRobot.Rules.Add(pravilo8);
                fsRobot.Rules.Add(pravilo9);
            }
            catch (Exception ex)
            {
                lbl1.Text = $"Greska! {ex}";
                lbl2.Text = $"";
            }
        }

        public double IzracunajSugeno(double udaljenost, double ugao)
        {
            try
            {
                var ulazneVrednosti = new Dictionary<FuzzyVariable, double> {
                    {fvUdaljenost, udaljenost},{fvUgao, ugao}
                };

                Dictionary<SugenoVariable, double> rezultat = fsRobot.Calculate(ulazneVrednosti);
                return rezultat[svSmerKretanja];
            }
            catch (Exception ex)
            {
                lbl1.Text = $"Greska! {ex}";
                lbl2.Text = $"";
            }
            return -1;
        }
    }
}