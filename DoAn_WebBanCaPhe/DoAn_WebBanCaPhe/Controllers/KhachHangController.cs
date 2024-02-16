using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_WebBanCaPhe.Models;

namespace DoAn_WebBanCaPhe.Controllers
{
    public class KhachHangController : Controller
    {
        //
        // GET: /KhachHang/
        QLQuanCaPheDataContext dl = new QLQuanCaPheDataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuLoaiSP()
        {
            List<LOAIMONNUOC> ds = dl.LOAIMONNUOCs.ToList();
            return PartialView(ds);
        }

        public ActionResult HTSP_TheoLoai(string id)
        {
            List<THUCUONG> ds = dl.THUCUONGs.Where(t => t.MALOAI == int.Parse(id)).ToList();
            ViewBag.maloai = dl.LOAIMONNUOCs.ToList();
            return View("SanPham", ds);
        }

        public ActionResult SanPham()
        {
            List<THUCUONG> ds = dl.THUCUONGs.ToList();
            ViewBag.maloai = dl.LOAIMONNUOCs.ToList();
            return View(ds);
        }

        public ActionResult Detail(string id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");
            else
            {
                THUCUONG t = dl.THUCUONGs.Where(f => f.MATU == int.Parse(id)).FirstOrDefault();

                return View(t);
            }
        }

        [HttpPost]
        public ActionResult SuaKH(KHACHHANG t, FormCollection fc)
        {
            //GioHang1 g = LayGioHang();
            //List<Item> ds = g.dssp;
            List<THUCUONG> ds = dl.THUCUONGs.ToList();
            ViewBag.maloai = dl.LOAIMONNUOCs.ToList();
            KHACHHANG l = dl.KHACHHANGs.FirstOrDefault(s => s.MAKH != int.Parse(fc["id"]) && s.SDT == fc["phone"]);
            if (l == null)
            {
                KHACHHANG ft = dl.KHACHHANGs.FirstOrDefault(s => s.MAKH == int.Parse(fc["id"]));
                ft.TENKH = fc["name"];
                ft.DIACHI = fc["address"];
                ft.SDT = fc["phone"];
                ft.GIOITINH = fc["gender"];

                UpdateModel(ft);
                dl.SubmitChanges();

                return View("SanPham", ds);
            }
            else
            {
                // Thông báo cho người dùng
                ViewBag.ErrorMessage = "Sửa thông tin không thành công!";

                return View("KhachHang"); // Trả về View với thông báo lỗi
            }
        }

        public ActionResult Cart()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            if (kh != null)
            {
                GioHang1 g = LayGioHang();
                List<Item> ds = g.dssp;

                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                ViewBag.kh = k;
                return View(ds);
            }
            else
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
        }

        public GioHang1 LayGioHang()
        {
            GioHang1 gio = (GioHang1)Session["gh"];
            if (gio == null)
                gio = new GioHang1();
            return gio;
        }
        public void LuuGioHang(GioHang1 gio)
        {
            Session["gh"] = gio;
        }
        public ActionResult ChonMua(string id, string sl)
        {
            KHACHHANG gio = (KHACHHANG)Session["kh"];
            if (gio == null)
            {
                TempData["LoginMessage"] = "1";
                return RedirectToAction("SanPham");
            }

            THUCUONG tu = dl.THUCUONGs.FirstOrDefault(t => t.MATU == int.Parse(id));
            int soluong = int.Parse(sl);
            if (soluong <= tu.SL)
            {
                GioHang1 g = LayGioHang();
                Item i = g.dssp.FirstOrDefault(t => t.maSP == int.Parse(id));
                if (i == null)
                {
                    Item x = new Item(id, soluong);
                    g.Them(x);
                }
                else
                {
                    i.soLuong += soluong;
                }
                LuuGioHang(g);
                return RedirectToAction("SanPham");
            }
            else
            {
                TempData["LoginMessage"] = "Quantity is not enough to satisfy !";
                return RedirectToAction("SanPham"); // Trả về View với thông báo lỗi
            }
        }

        public ActionResult Xoa(string id)
        {
            GioHang1 g = LayGioHang();
            Item i = g.dssp.FirstOrDefault(t => t.maSP == int.Parse(id));
            if (i != null)
            {
                g.Xoa(i.maSP.ToString());
            }
            LuuGioHang(g);
            return RedirectToAction("Cart");
        }

        public ActionResult SLSP()
        {
            int sl = 0;
            GioHang1 gh = (GioHang1)Session["gh"];
            if(gh != null)
            {
                sl = gh.TongSL();
            }
            ViewBag.sl = sl;

            return PartialView();
        }

        [HttpGet]
        public ActionResult CapNhatSL(string masp, string sl)
        {
            
            // Lấy giỏ hàng từ Session
            var gio = Session["gh"] as GioHang1;

            if (gio == null)
            {
                // Nếu giỏ hàng chưa tồn tại, khởi tạo giỏ hàng mới
                gio = new GioHang1();
                Session["gh"] = gio;
            }

            // Lặp qua từng sản phẩm trong giỏ hàng
            foreach (var i in gio.dssp)
            {
                if (i.maSP == int.Parse(masp))
                {
                    i.soLuong = int.Parse(sl);
                    LuuGioHang(gio);
                    // Thêm dòng log để xác nhận action đã được gọi
                    //Console.WriteLine("CapNhatSoLuong action has been called.");
                    //return Json(new { success = true });
                    return RedirectToAction("Cart");
                }
            }

            return RedirectToAction("Cart");
        }

        public ActionResult XacNhanThanhToan()
        {
            GioHang1 gio = (GioHang1)Session["gh"];
            if (gio == null)
            {
                TempData["LoginMessage"] = "1";
                return RedirectToAction("Cart");
            }

            bool flag = true; //kiểm tra số lượng của trong giỏ hàng có nhỏ hơn sl sẳn có không

            foreach (Item i in gio.dssp)
            {
                THUCUONG tu = dl.THUCUONGs.SingleOrDefault(t => t.MATU == i.maSP);
                if (i.soLuong > tu.SL)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                KHACHHANG kh = (KHACHHANG)Session["kh"];
                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                ViewBag.kh = k;


                HOADON hd = new HOADON();

                hd.NGAYLAP = DateTime.Now;
                hd.MAKH = kh.MAKH;
                hd.TONGTIEN = gio.TongTien();
                hd.TRANGTHAI = false;
                dl.HOADONs.InsertOnSubmit(hd);
                dl.SubmitChanges();

                foreach (Item i in gio.dssp)
                {
                    CHITIET_THUCUONG ct = new CHITIET_THUCUONG();
                    ct.MAHD = hd.MAHD;
                    ct.MATU = i.maSP;
                    ct.SL = i.soLuong;
                    ct.GIABAN = i.donGia;
                    ct.THANHTIEN = i.thanhTien;
                    dl.CHITIET_THUCUONGs.InsertOnSubmit(ct);
                    dl.SubmitChanges();

                    //THUCUONG tu = dl.THUCUONGs.SingleOrDefault(t => t.MATU == i.maSP);
                    //if (tu != null)
                    //{
                    //    tu.SL -= i.soLuong;
                    //    dl.SubmitChanges();
                    //}

                }

                VANCHUYEN vc = new VANCHUYEN();
                vc.MAHD = hd.MAHD;
                vc.DIACHI = k.DIACHI;
                vc.SODIENTHOAI = k.SDT;
                dl.VANCHUYENs.InsertOnSubmit(vc);
                dl.SubmitChanges();

                return View();
            }
            else
            {
                TempData["LoginMessage"] = "Quantity is not enough to satisfy !";
                return RedirectToAction("SanPham"); // Trả về View với thông báo lỗi
            }
            
        }

        public ActionResult KhachHang()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            if (kh != null)
            {
                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                return View(k);
            }
            else
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

        }
        public ActionResult KiemTraPass()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
            return View(k);
        }
        [HttpPost]
        public ActionResult KiemTraPass(FormCollection fc)
        {
            string user = fc["username"];
            string pass = fc["password"];
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            if(user == kh.TENDANGNHAP && pass == kh.ACCOUNT.MATKHAU)
            {
                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                return View("DoiPass", k);
            }
            else
            {
                TempData["LoginMessage"] = "Invalid password !";

                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                return RedirectToAction("KiemTraPass", k); // Trả về View với thông báo lỗi
            }
        }

        public ActionResult DoiPass()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
            return View(k);
        }
        [HttpPost]
        public ActionResult DoiPass(FormCollection fc)
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            string user = fc["username"];
            if (fc["password"] == fc["confirmpassword"])
            {
                ACCOUNT a = dl.ACCOUNTs.FirstOrDefault(f => f.TENDANGNHAP == user);
                if (a != null)
                {
                    a.TENDANGNHAP = user;
                    a.MATKHAU = fc["password"];
                    a.TRANGTHAI = true;
                    UpdateModel(a);
                    dl.SubmitChanges();

                    TempData["LoginMessage"] = "Password changed successfully";
                    KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                    return View("KhachHang", k); // Trả về View với thông báo lỗi
                }
                else
                {
                    // Thông báo cho người dùng
                    ViewBag.ErrorMessage = "Password change failed !";

                    KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                    return View("KhachHang", k); // Trả về View với thông báo lỗi
                }
            }
            else
            {
                TempData["LoginMessage"] = "Passwords do not match !";

                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                return View("DoiPass", k); // Trả về View với thông báo lỗi
            }
            
            
        }
        public ActionResult DonHang()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            if (kh != null)
            {
                KHACHHANG k = dl.KHACHHANGs.FirstOrDefault(f => f.TENDANGNHAP == kh.TENDANGNHAP);
                List<VANCHUYEN> vc = dl.VANCHUYENs.Where(f => f.HOADON.MAKH == k.MAKH).ToList();
                return View(vc);
            }
            else
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

        }
    }
}
