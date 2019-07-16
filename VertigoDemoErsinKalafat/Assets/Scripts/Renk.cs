using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renk : MonoBehaviour
{
    private Vector2 ilkDokunulan;
    private Vector2 sonDokunulan;
    private Vector2 geciciKonum;
    public float suruklemeAcisi = 0;
    public float suruklemeEngelle = 1f;

    public int sutun;
    public int satir;
    public int oncekiSutun;
    public int oncekiSatir;
    public int hedefX;
    public int hedefY;
    private GameObject digerRenk;
    private OyunTahtasi oyunTahtasi;

    public bool eslesme = false;

    void Start()
    {
        oyunTahtasi = FindObjectOfType<OyunTahtasi>();
        hedefX = (int)transform.position.x;
        hedefY = (int)transform.position.y;
        satir = hedefY;
        sutun = hedefX;
        oncekiSatir = satir;
        oncekiSutun = sutun;
    }

    void Update()
    {
        EslesmeBul();
        if (eslesme)
        {
            SpriteRenderer eslestirildi = GetComponent<SpriteRenderer>();
            eslestirildi.color = new Color(1f, 1f, 1f, .2f);
        }

        hedefX = sutun;
        hedefY = satir;
        if (Mathf.Abs(hedefX - transform.position.x) > .1)
        {
            geciciKonum = new Vector2(hedefX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, geciciKonum, .6f);
            if(oyunTahtasi.tumRenkler[sutun,satir] != this.gameObject) {
                oyunTahtasi.tumRenkler[sutun, satir] = this.gameObject;
            }
        }
        else 
        {
            geciciKonum = new Vector2(hedefX, transform.position.y);
            transform.position = geciciKonum;
            oyunTahtasi.tumRenkler[sutun, satir] = this.gameObject;
        }

        if (Mathf.Abs(hedefY - transform.position.y) > .1)
        {
            geciciKonum = new Vector2(transform.position.x, hedefY);
            transform.position = Vector2.Lerp(transform.position, geciciKonum, .6f);
            if (oyunTahtasi.tumRenkler[sutun, satir] != this.gameObject) {
                oyunTahtasi.tumRenkler[sutun, satir] = this.gameObject;
            }
        }
        else
        {
            geciciKonum = new Vector2(transform.position.x, hedefY);
            transform.position = geciciKonum;
            //oyunTahtasi.tumRenkler[sutun, satir] = this.gameObject;
        }

    }

    public IEnumerator Kontrol()
    {
        yield return new WaitForSeconds(.5f);
        if (digerRenk != null) {
            if (!eslesme && !digerRenk.GetComponent<Renk>().eslesme)
            {
                digerRenk.GetComponent<Renk>().satir = satir;
                digerRenk.GetComponent<Renk>().sutun = sutun;
                satir = oncekiSatir;
                sutun = oncekiSutun;
            }
            else {
                oyunTahtasi.TumEslesmeleriSil();
            }
            digerRenk = null;
        }
    }

    private void OnMouseDown()
    {
        ilkDokunulan = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        sonDokunulan = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HesaplananAci();
    }

    void HesaplananAci()
    {
        if (Mathf.Abs(sonDokunulan.y - ilkDokunulan.y) > suruklemeEngelle || Mathf.Abs(sonDokunulan.x - ilkDokunulan.x) > suruklemeEngelle)
        {
            suruklemeAcisi = Mathf.Atan2(sonDokunulan.y - ilkDokunulan.y, sonDokunulan.x - ilkDokunulan.x) * 180/ Mathf.PI;
            ParcalariTasi();
        }
        

    }

    void ParcalariTasi()
    {
        if (suruklemeAcisi > -45 && suruklemeAcisi <= 45 && sutun < oyunTahtasi.genislik - 1)
        {   // sağa kaydır
            digerRenk = oyunTahtasi.tumRenkler[sutun + 1, satir];
            digerRenk.GetComponent<Renk>().sutun--;
            sutun++;
        }
        else if (suruklemeAcisi > 45 && suruklemeAcisi <= 135 && satir < oyunTahtasi.yukseklik - 1)
        {   // yukarı kaydır
            digerRenk = oyunTahtasi.tumRenkler[sutun, satir + 1];
            digerRenk.GetComponent<Renk>().satir--;
            satir++;
        }
        else if ((suruklemeAcisi > 135 || suruklemeAcisi <= -135) && sutun > 0)
        {   // sola kaydır
            digerRenk = oyunTahtasi.tumRenkler[sutun - 1, satir];
            digerRenk.GetComponent<Renk>().sutun++;
            sutun--;
        }
        else if (suruklemeAcisi < -45 && suruklemeAcisi >= -135 && satir > 0)
        {   // aşağı kaydır
            digerRenk = oyunTahtasi.tumRenkler[sutun, satir - 1];
            digerRenk.GetComponent<Renk>().satir++;
            satir--;
        }
        StartCoroutine(Kontrol());
    }


    void EslesmeBul()
    {
        if (sutun > 0 && sutun < oyunTahtasi.genislik - 1)
        {
            GameObject solRenk1 = oyunTahtasi.tumRenkler[sutun - 1, satir];
            GameObject sagRenk1 = oyunTahtasi.tumRenkler[sutun + 1, satir];
            if (solRenk1 != null && sagRenk1 != null)
            {
                if (solRenk1.tag == this.gameObject.tag && sagRenk1.tag == this.gameObject.tag)
                {
                    solRenk1.GetComponent<Renk>().eslesme = true;
                    sagRenk1.GetComponent<Renk>().eslesme = true;
                    eslesme = true;
                }
            }
            
        }

        if (satir > 0 && satir < oyunTahtasi.yukseklik - 1)
        {
            GameObject ustRenk1 = oyunTahtasi.tumRenkler[sutun, satir + 1];
            GameObject altRenk1 = oyunTahtasi.tumRenkler[sutun, satir - 1];
            if (ustRenk1 != null && altRenk1 != null)
            {
                if (ustRenk1.tag == this.gameObject.tag && altRenk1.tag == this.gameObject.tag)
                {
                    ustRenk1.GetComponent<Renk>().eslesme = true;
                    altRenk1.GetComponent<Renk>().eslesme = true;
                    eslesme = true;
                }
            }
            
        }
    }
}