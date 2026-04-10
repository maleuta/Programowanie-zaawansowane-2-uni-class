using System;
using System.Collections.Generic;

// 1 ////////////////////////////
public abstract class PosiadaczRachunku
{
    public abstract override string ToString();
}


// 2 zadanie /////////////////////////
public class OsobaFizyczna : PosiadaczRachunku
{
    private string imie;
    private string nazwisko;
    private string drugieImie;
    private string? pesel;
    private string? numerPaszportu;

    public string Imie
    {
        get => imie;
        set => imie = value;
    }

    public string Nazwisko
    {
        get => nazwisko;
        set => nazwisko = value;
    }

    public string DrugieImie
    {
        get => drugieImie;
        set => drugieImie = value;
    }


// dod. 4 ////////////////////
    public string? PESEL
    {
        get => pesel;
        set
        {
        if (value == null || value.Length == 11)
            pesel = value;
        else
            throw new Exception("PESEL musi mieć dokładnie 11 cyfr lub być null.");
        }
    }

    public string? NumerPaszportu
    {
        get => numerPaszportu;
        set => numerPaszportu = value;
    }


// 3 zadanie //////////////
    public OsobaFizyczna(string imie, string nazwisko, string drugieImie, string pesel, string? numerPaszportu)
    {
    if ((pesel == null || pesel.Length != 11) && numerPaszportu == null)
    {
        throw new Exception("PESEL musi mieć 11 cyfr lub numer paszportu musi być nie null");
    }
    this.imie = imie;
    this.nazwisko = nazwisko;
    this.drugieImie = drugieImie;
    this.PESEL = pesel;
    this.numerPaszportu = numerPaszportu;
    }

//////////////////////////////////

    public override string ToString()
    {
        return $"Osoba fizyczna: {Imie} {Nazwisko}";
    }
}

// 4 //////////////////////////////////////////
public class OsobaPrawna : PosiadaczRachunku
{
    private string nazwa;
    private string siedziba;

    public string Nazwa { get { return nazwa; } }
    public string Siedziba { get { return siedziba; } }

// 5 ///
    public OsobaPrawna(string nazwa, string siedziba)
    {
        this.nazwa = nazwa;
        this.siedziba = siedziba;
    }

/////

    public override string ToString()
    {

        return $"Osoba prawna: {nazwa}, siedziba: {siedziba}";
    }
}

// 8 ///////////////////////
// RachunekBankowy korzysta z Transakcji na swojej liście
public class Transakcja
{
    private RachunekBankowy? rachunekZrodlowy;
    private RachunekBankowy? rachunekDocelowy;
    private decimal kwota;
    private string opis;

    public RachunekBankowy? RachunekZrodlowy { 
        get => rachunekZrodlowy;  
        set => rachunekZrodlowy = value; } 
    public RachunekBankowy? RachunekDocelowy { 
        get => rachunekDocelowy; 
        set => rachunekDocelowy = value; } 
    public decimal Kwota { 
        get => kwota; 
        set => kwota = value; } 
    public string Opis { 
        get => opis; 
        set => opis = value; } 



// 9 ////////////////////////////
    public Transakcja(RachunekBankowy? rachunekZrodlowy, RachunekBankowy? rachunekDocelowy, decimal kwota, string opis)
    {
        if (rachunekZrodlowy == null && rachunekDocelowy == null)
        {
            throw new Exception("Rachunek docelowy i źródłowy nie mogą być równocześnie null.");
        }
        this.rachunekZrodlowy = rachunekZrodlowy;
        this.rachunekDocelowy = rachunekDocelowy;
        this.kwota = kwota;
        this.opis = opis;
    }


// dod. 1 /////////////////////
    public override string ToString()
    {
    string nrZrodlowy = rachunekZrodlowy != null ? rachunekZrodlowy.Numer : "gotówka";
    string nrDocelowy = rachunekDocelowy != null ? rachunekDocelowy.Numer : "gotówka";
    return $"Transakcja: od {nrZrodlowy} do {nrDocelowy}, kwota: {kwota}, opis: {opis}";
    }
}

// 6 //////////////////
public class RachunekBankowy
{
    private string numer;
    private decimal stanRachunku;
    private bool czyDozwolonyDebet;
    private List<PosiadaczRachunku> posiadaczeRachunku = new List<PosiadaczRachunku>();
    private List<Transakcja> transakcje = new List<Transakcja>(); // p 10

    public string Numer { 
        get => numer; 
        set => numer = value; } 
    public decimal StanRachunku { 
        get => stanRachunku; 
        set => stanRachunku = value;  }
    public bool CzyDozwolonyDebet { 
        get => czyDozwolonyDebet;  
        set => czyDozwolonyDebet = value; }
    public List<PosiadaczRachunku> PosiadaczeRachunku { 
        get => posiadaczeRachunku;
        set => posiadaczeRachunku = value; } 
    public List<Transakcja> Transakcje {  // p.10
        get =>  transakcje;  
        set => transakcje = value; }


/// 7 ///////////////////////////////////
    public RachunekBankowy(string numer, decimal stanRachunku, bool czyDozwolonyDebet, List<PosiadaczRachunku> posiadaczeRachunku)
    {
        if (posiadaczeRachunku == null || posiadaczeRachunku.Count == 0)
        {
            throw new Exception("Rachunek musi mieć co najmniej jednego właściciela.");
        }

        this.numer = numer;
        this.stanRachunku = stanRachunku;
        this.czyDozwolonyDebet = czyDozwolonyDebet;
        this.posiadaczeRachunku = posiadaczeRachunku;
    }

    // 10 //////////////////////////////
    public static void DokonajTransakcji(RachunekBankowy zrodlowy, RachunekBankowy docelowy, decimal kwota, string opis)
    {
        if (kwota < 0)
        {
            throw new Exception("Kwota transakcji nie może być ujemna.");
        }

        if (zrodlowy == null && docelowy == null)
        {
            throw new Exception("Oba rachunki są równe null.");
        }

        if (zrodlowy != null && !zrodlowy.CzyDozwolonyDebet && (kwota > zrodlowy.StanRachunku))
        {
            throw new Exception("Brak wystarczających środków na rachunku źródłowym (debet niedozwolony).");
        }

        Transakcja nowaTransakcja = new Transakcja(zrodlowy, docelowy, kwota, opis);

        if (zrodlowy == null)
        {
            // gotówkowa (+)
            docelowy.StanRachunku += kwota;
            docelowy.Transakcje.Add(nowaTransakcja);
        }
        else if (docelowy == null)
        {
            // gotówkowa (-)
            zrodlowy.StanRachunku -= kwota;
            zrodlowy.Transakcje.Add(nowaTransakcja);
        }
        else
        {
            // przelew
            zrodlowy.StanRachunku -= kwota;
            docelowy.StanRachunku += kwota;
            zrodlowy.Transakcje.Add(nowaTransakcja);
            docelowy.Transakcje.Add(nowaTransakcja);
        }
    }


// dod. 2 ////////////////////////////////////////////
    public static RachunekBankowy operator +(RachunekBankowy rachunek, PosiadaczRachunku posiadacz)
    {
    if (rachunek.posiadaczeRachunku.Contains(posiadacz))
        throw new Exception("Posiadacz już jest na liście posiadaczy rachunku.");
    rachunek.posiadaczeRachunku.Add(posiadacz);
    return rachunek;
    }

    public static RachunekBankowy operator -(RachunekBankowy rachunek, PosiadaczRachunku posiadacz)
    {
    if (!rachunek.posiadaczeRachunku.Contains(posiadacz))
        throw new Exception("Posiadacza nie ma na liście posiadaczy rachunku.");
    if (rachunek.posiadaczeRachunku.Count == 1)
        throw new Exception("Liczba posiadaczy nie może spaść poniżej 1.");
    rachunek.posiadaczeRachunku.Remove(posiadacz);
    return rachunek;
    }


// dod. 3 ///////////////////////////////
    public override string ToString()
    {
    return $"Rachunek: {numer}\n" +
           $"Stan: {stanRachunku}\n" +
           $"Posiadacze: {string.Join(", ", posiadaczeRachunku)}\n" +
           $"Transakcje:\n{string.Join("\n", transakcje)}";
    }
}