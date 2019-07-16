using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OyunTahtasi : MonoBehaviour
{
    public int genislik;
    public int yukseklik;
    public GameObject parcalarDuzlemi;
    private Arkaplan[,] parcalar;
    public GameObject[] renkler;
    public GameObject[,] tumRenkler;

    void Start()
    {
        parcalar = new Arkaplan[genislik, yukseklik];
        tumRenkler = new GameObject[genislik, yukseklik];
        Olustur();
    }

    private void Olustur()
    {
        for (int i = 0; i < genislik; i++)
        {
            for (int j = 0; j < yukseklik; j++)
            {
                Vector2 geciciKonum = new Vector2(i, j);
                GameObject Arkaplan = Instantiate(parcalarDuzlemi, geciciKonum, Quaternion.identity) as GameObject;
                Arkaplan.transform.parent = this.transform;
                Arkaplan.name = "(" + i + "," + j + ")";
                int kullanilacakRenk = Random.Range(0, renkler.Length);
                int maxTekrar = 0;
                while (Eslesmis(i, j, renkler[kullanilacakRenk]) && maxTekrar < 100)
                {
                    kullanilacakRenk = Random.Range(0, renkler.Length);
                    maxTekrar++;
                }
                maxTekrar = 0;

                GameObject renk = Instantiate(renkler[kullanilacakRenk], geciciKonum, Quaternion.identity);
                renk.transform.parent = this.transform;
                renk.name = "(" + i + "," + j + ")";
                tumRenkler[i, j] = renk;
            }
        }

    }

    void Update()
    {

    }

    private bool Eslesmis(int sutun, int satir, GameObject parca) // oyunda eşleşmiş renkler olmamalı
    {
        if (sutun > 1 && satir > 1)
        {
            if (tumRenkler[sutun - 1, satir].tag == parca.tag && tumRenkler[sutun - 2, satir].tag == parca.tag)
            {
                return true;
            }
            if (tumRenkler[sutun, satir - 1].tag == parca.tag && tumRenkler[sutun, satir - 2].tag == parca.tag)
            {
                return true;
            }
        }
        else if (sutun <= 1 || satir <= 1)
        {
            if (satir > 1)
            {
                if (tumRenkler[sutun, satir - 1].tag == parca.tag && tumRenkler[sutun, satir - 2].tag == parca.tag)
                {
                    return true;
                }
            }
            if (sutun > 1)
            {
                if (tumRenkler[sutun - 1, satir].tag == parca.tag && tumRenkler[sutun - 2, satir].tag == parca.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void EslesmeSil(int sutun, int satir)
    {
        if (tumRenkler[sutun, satir].GetComponent<Renk>().eslesme)
        {
            Destroy(tumRenkler[sutun, satir]);
            tumRenkler[sutun, satir] = null;
        }
    }

    public void TumEslesmeleriSil()
    {
        for (int i = 0; i < genislik; i++)
        {
            for (int j = 0; j < yukseklik; j++)
            {
                if (tumRenkler[i, j] != null)
                {
                    EslesmeSil(i, j);
                }
            }
        }
        StartCoroutine(SatirEksilt());
    }

    private IEnumerator SatirEksilt()
    {
        int nullMiktari = 0;
        for (int i = 0; i < genislik; i++)
        {
            for (int j = 0; j < yukseklik; j++)
            {
                if (tumRenkler[i, j] == null)
                {
                    nullMiktari++;
                }
                else if (nullMiktari > 0)
                {
                    tumRenkler[i, j].GetComponent<Renk>().satir -= nullMiktari;
                    tumRenkler[i, j] = null;
                }
            }
            nullMiktari = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(TekrarDoldurCalistir());
    }

    private void TekrarDoldur()
    {
        for (int i = 0; i < genislik; i++)
        {
            for (int j = 0; j < yukseklik; j++)
            {
                if (tumRenkler[i, j] == null)
                {
                    Vector2 geciciKonum = new Vector2(i, j);
                    int kullanilanRenk = Random.Range(0, renkler.Length);
                    GameObject parca = Instantiate(renkler[kullanilanRenk], geciciKonum, Quaternion.identity);
                    tumRenkler[i, j] = parca;
                }
            }
        }
    }

    private bool OyundakiEslesmeler()
    {
        for (int i = 0; i < genislik; i++) {
            for (int j = 0; j < yukseklik; j++) {
                if (tumRenkler[i, j] != null) {
                    if (tumRenkler[i, j].GetComponent<Renk>().eslesme) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator TekrarDoldurCalistir()
    {
        TekrarDoldur();
        yield return new WaitForSeconds(.5f);

        while (OyundakiEslesmeler())
        {
            yield return new WaitForSeconds(.5f);
            TumEslesmeleriSil();
        }
    }
}