﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Rumrum_UrunDuzenleme : System.Web.UI.Page
{
    cls_Admin_Urunler U = new cls_Admin_Urunler();
    public static int GelID;
    public static string GeliyorID;
    [ScriptMethod()]
    [WebMethod]
    public static List<string> SearchCustomers(string prefixText, int count)
    {
        using (SqlConnection conn = new SqlConnection(Yol.ECon))
        {
            using (SqlCommand cmd = new SqlCommand("select UrunAdi from E_Urunler where IsActive=1 and UrunAdi like '%" + prefixText + "%'", conn))
            {
                //  cmd.Parameters.AddWithValue("@SearchText", prefixText);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                List<string> customers = new List<string>();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        customers.Add(sdr["UrunAdi"].ToString());
                    }
                }
                conn.Close();
                return customers;
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["eticaret"] != null)
            {
                if (!IsPostBack)
                {
                    Label label = Master.FindControl("UrlMaplbl") as Label;
                    label.Text = "Ürün Düzenleme";
                    Label label2 = Master.FindControl("TepeMesajLbl") as Label;
                    label2.Text = "Ürün Düzenleme İşlemleri";
                    if (Request.QueryString["ID"] != null)
                    {
                        GeliyorID = Ortak.Decrypt(Request.QueryString["ID"].ToString());
                        HataLbl.Visible = false;
                        Urunyok.Visible = false;
                        Details.Visible = true;
                        Urun_Bilgileri_Getir(GeliyorID);
                        UrunAdiBox.Text = UrunAdiLbl.Text;
                    }
                    else
                    {
                        Urun_Bilgileri_Getir(GelID.ToString());
                    }
                }
            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }
        }
        catch (Exception)
        {
            HataVar.Visible = true;
        }
    }
    public void Urun_Bilgileri_Getir(string _Urun_Aydim)
    {
        string[] Gele_Teslimat = U.Urun_Arama_Turn_Details(_Urun_Aydim);
        if (!string.IsNullOrEmpty(Gele_Teslimat[0]))
        {
            UrunAdiLbl.Text = Gele_Teslimat[0].ToString();
            UrunResim.ImageUrl = "../Urunler/Logo/" + Gele_Teslimat[1].ToString();
            EskiFiyatLbl.Text = Gele_Teslimat[2].ToString();
            YeniFiyatlbl.Text = Gele_Teslimat[3].ToString();
            KategoriAdiLbl.Text = Gele_Teslimat[4].ToString();
            TelefonLbl.Text = Gele_Teslimat[5].ToString();
            ModelLbl.Text = Gele_Teslimat[6].ToString();
        }
    }
    protected void urunBilgileriniGetirBtn_Click(object sender, EventArgs e)
    {
        try
        {
            GelID = U.Urun_Arama_Turn_ID(UrunAdiBox.Text);
            if (GelID > 0)
            {
                Urun_Bilgileri_Getir(GelID.ToString());
                HataLbl.Visible = false;
                Urunyok.Visible = false;
                Details.Visible = true;
            }
            else
            {
                HataLbl.Visible = false;
                Urunyok.Visible = true;
                Details.Visible = false;
            }
        }
        catch (Exception)
        {
            HataLbl.Visible = true;
        }
    }
    protected void DuzenleBtn_Click(object sender, EventArgs e)
    {
        if (GelID!=0)
        {
            string Git_URL = "UrunDuzenlemeOnay.aspx?OID=" + Ortak.Encrypt(GelID.ToString());
            Response.Redirect(Git_URL);
        }
        else
        {
            string Git_URL = "UrunDuzenlemeOnay.aspx?OID=" + Ortak.Encrypt(GeliyorID.ToString());
            Response.Redirect(Git_URL);
        }
    }
}