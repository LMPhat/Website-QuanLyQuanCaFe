using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_WebBanCaPhe.Models;

namespace DoAn_WebBanCaPhe.Controllers
{
    public class TaiKhoanController : Controller
    {
        //
        // GET: /TaiKhoan/
        QLQuanCaPheDataContext dl = new QLQuanCaPheDataContext();

        public ActionResult KhoiTao()
        {
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            if (kh != null)
                ViewBag.chao = "Hello, " + kh.TENKH;
            return PartialView();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            try
            {
                ACCOUNT tk = dl.ACCOUNTs.FirstOrDefault(a => a.TENDANGNHAP == fc["username"] && a.MATKHAU == fc["password"] && a.TRANGTHAI == true);

                if (tk != null)
                {
                    KHACHHANG kh = dl.KHACHHANGs.FirstOrDefault(t => t.TENDANGNHAP == tk.TENDANGNHAP);

                    if (kh != null)
                    {
                        Session["kh"] = kh;
                        return RedirectToAction("SanPham", "KhachHang");
                    }
                }
                else
                {
                    TempData["LoginMessage"] = "Tài khoản của bạn không tồn tại!";
                    return View("Login"); // Trả về View với thông báo lỗi
                }
            }
            catch (Exception e)
            {
                // Xử lý ngoại lệ ở đây
                // Ví dụ: Ghi log, thông báo lỗi, hoặc xử lý ngoại lệ theo cách khác
                TempData["LoginMessage"] = "Đã xảy ra lỗi khi đăng nhập!";
                return View("Login"); // Trả về View với thông báo lỗi
            }
            
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login", "TaiKhoan");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(KHACHHANG d, ACCOUNT a, FormCollection fc)
        {
            if (fc["password"] == fc["confirmPassword"])
            {
                ACCOUNT ktr = KTraTenDangNhap(fc["username"].Trim());
                if (ktr == null)
                {
                    a.TENDANGNHAP = fc["username"];
                    a.MATKHAU = fc["password"];
                    a.TRANGTHAI = true;
                    dl.ACCOUNTs.InsertOnSubmit(a);
                    dl.SubmitChanges();

                    d.TENKH = fc["name"];
                    d.SDT = fc["phone"];
                    d.DIACHI = fc["address"];
                    string gt = fc["gender"];
                    if (gt == null)
                        d.GIOITINH = "NAM";
                    else
                        d.GIOITINH = gt;
                    d.TENDANGNHAP = fc["username"];
                    dl.KHACHHANGs.InsertOnSubmit(d);
                    dl.SubmitChanges();
                    return RedirectToAction("Login", "TaiKhoan");
                }
                else
                {
                    TempData["LoginMessage"] = "This account has already existed!";
                    return View("Register"); // Trả về View với thông báo lỗi
                }
            }
            else
            {
                TempData["LoginMessage"] = "Passwords do not match!";
                return View("Register"); // Trả về View với thông báo lỗi
            }
        }

        ACCOUNT KTraTenDangNhap(string tenDN)
        {
            ACCOUNT acc = dl.ACCOUNTs.Where(f=> f.TENDANGNHAP == tenDN).FirstOrDefault();
            return acc;
        }

    }
}
