// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;
using System.Xml.Linq;


int musteriNumarasi = 15000000;

CalistirmaMotoru.KomutCalistir(NameConstants.MuhasebeModulu, NameConstants.MaasYatir, new object[] { musteriNumarasi, true });

CalistirmaMotoru.KomutCalistir(NameConstants.MuhasebeModulu,NameConstants.YillikUcretTahsilEt, new object[] { musteriNumarasi, true });

CalistirmaMotoru.KomutCalistir(NameConstants.MuhasebeModulu, NameConstants.YillikUcretTahsilEt, new object[] { musteriNumarasi, false });

CalistirmaMotoru.BekleyenIslemleriGerceklestir();



public class CalistirmaMotoru
{

    public static object[] KomutCalistir(string modulSinifAdi, string methodAdi, object[] inputs)
    {
        if (!checkParameters(inputs))
        {
            return inputs;
        }

        if (inputs.Length > 1 && (bool)inputs[1])
        {
            new VeritabaniIslemleri().AddWaitingJob(modulSinifAdi, methodAdi, (int)inputs[0]);
        }
        else
        {
            new CalistirmaMotoru().InvokeMethod(modulSinifAdi, methodAdi, (int)inputs[0]);
        }
        return inputs;
    }

    public static void BekleyenIslemleriGerceklestir()
    {
        VeritabaniIslemleri veritabanıİşlemleri = new VeritabaniIslemleri();
        var waitingJobs = veritabanıİşlemleri.GetWaitingJobs();

        foreach (var item in waitingJobs)
        {
            new CalistirmaMotoru().InvokeMethod(item.modulSinifAdi, item.methodAdi, item.musteriNumarasi);
            item.jobState = true;
            Console.WriteLine("{0} numaralı müşterinin {1} bekleyen işlemi yapıldı.", item.musteriNumarasi, item.methodAdi);

        }
    }
    private static bool checkParameters(object[] inputs)
    {
        bool checkResult = true;
        if (inputs.Length == 0 || inputs.Length > 2)
        {
            checkResult = false;
        }
        if (inputs[0].GetType() != typeof(int))
        {
            checkResult = false;
        }
        if (inputs.Length > 1 && inputs[1].GetType() != typeof(bool))
        {
            checkResult = false;
        }
        if (!checkResult)
            Console.WriteLine("İnput parametreleri sırasıyla müşteri numarası ve bekleyen işlem mi olacak şekilde int ve bool tipinde olmalıdır.");
        return checkResult;
    }
    private void InvokeMethod(string modulSinifAdi, string methodAdi, int musteriNumarasi)
    {
        Type type = Type.GetType(modulSinifAdi, true);
        object instance = Activator.CreateInstance(type);
        var method = type.GetTypeInfo().GetDeclaredMethod(methodAdi);
        method.Invoke(instance, new object[] { musteriNumarasi });
    }
}

public class MuhasebeModulu
{
    private void MaasYatir(int musteriNumarasi)
    {
        // gerekli işlemler gerçekleştirilir.
        Console.WriteLine(string.Format("{0} numaralı müşterinin maaşı yatırıldı.", musteriNumarasi));
    }

    private void YillikUcretTahsilEt(int musteriNumarasi)
    {
        // gerekli işlemler gerçekleştirilir.
        Console.WriteLine("{0} numaralı müşteriden yıllık kart ücreti tahsil edildi.", musteriNumarasi);
    }

    private void OtomatikOdemeleriGerceklestir(int musteriNumarasi)
    {
        // gerekli işlemler gerçekleştirilir.
        Console.WriteLine("{0} numaralı müşterinin otomatik ödemeleri gerçekleştirildi.", musteriNumarasi);
    }
}

public class VeritabaniIslemleri
{
    public static List<WaitingJobModel> waitingJobModels = new List<WaitingJobModel>();

    public void AddWaitingJob(string modulSinifAdi, string methodAdi, int musteriNumarasi)
    {
        waitingJobModels.Add(new WaitingJobModel
        {
            musteriNumarasi = musteriNumarasi,
            methodAdi = methodAdi,
            modulSinifAdi = modulSinifAdi
        });

        Console.WriteLine("{0} numaralı müşterinin {1} işlemi bekleyen işlemlere alındı.", musteriNumarasi, methodAdi);

    }
   
    public List<WaitingJobModel> GetWaitingJobs()
    {
        return waitingJobModels.Where(x=>!x.jobState).ToList();
    }
}

public class WaitingJobModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string modulSinifAdi { get; set; }
    public string methodAdi { get; set; }
    public int musteriNumarasi { get; set; }
    public bool jobState { get; set; } = false;
}

public class NameConstants
{
    public const string MuhasebeModulu = "MuhasebeModulu";
    public const string MaasYatir = "MaasYatir";
    public const string YillikUcretTahsilEt = "YillikUcretTahsilEt";
    public const string OtomatikOdemeleriGerceklestir = "OtomatikOdemeleriGerceklestir";

}