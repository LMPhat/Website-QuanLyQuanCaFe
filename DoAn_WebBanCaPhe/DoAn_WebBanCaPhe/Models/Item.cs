using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_WebBanCaPhe.Models
{
    public class Item
    {
        QLQuanCaPheDataContext dl = new QLQuanCaPheDataContext();
        public int maSP { get; set; }
        public string tenSP { get; set; }
        public string anhBia { get; set; }
        public int donGia { get; set; }
        public int soLuong { get; set; }

        public int thanhTien { get { return soLuong * donGia; } }


        public Item(string ms)
        {
            maSP = int.Parse(ms);
            THUCUONG s = dl.THUCUONGs.FirstOrDefault(t => t.MATU == maSP);
            tenSP = s.TENTU;
            anhBia = s.HINHANH;
            donGia = int.Parse(s.DONGIA.ToString());

            soLuong = 1;
        }
        public Item(string ms, int sl)
        {
            maSP = int.Parse(ms);
            THUCUONG s = dl.THUCUONGs.FirstOrDefault(t => t.MATU == maSP);
            tenSP = s.TENTU;
            anhBia = s.HINHANH;
            donGia = int.Parse(s.DONGIA.ToString());
            soLuong = sl;
        }

    }

    public class GioHang1
    {
        public List<Item> dssp;
        public GioHang1()
        {
            dssp = new List<Item>();
        }
        public void Them(Item x)
        {
            dssp.Add(x);
        }

        public int SLMatHang()
        {
            return dssp.Count();
        }
        public int TongSL()
        {
            return dssp.Sum(t => t.soLuong);
        }
        public int TongTien()
        {
            return dssp.Sum(t => t.thanhTien);
        }
        public int Xoa(string ma)
        {
            Item sp = dssp.Find(n => n.maSP == int.Parse(ma));
            if (sp != null)
            {
                dssp.Remove(sp);
                return 1;
            }
            return -1;

        }
        
        public int XoaAll()
        {
            if (dssp != null)
            {
                dssp.Clear();
                return 1;
            }
            return -1;

        }
    }
}