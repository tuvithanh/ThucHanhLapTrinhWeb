using _23DH113217_MyStore.Models.ViewModel;
using _23DH113217_MyStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _23DH113217_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities(); // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //Kiểm tra giỏ hàng trong session,
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) { return RedirectToAction("Login", "Account"); }
            //Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang Đăng nhập //nếu có rồi thì lấy địa chỉ của khách hàng và gán vào ShippingAddress của CheckoutVm
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }
            var model = new CheckoutVM
            {
                CartItems = cart,
                TotalAmount = cart.Sum(item => item.TotalPrice),
                OrderDate = DateTime.Now,
                ShippingAddress = customer.CustomerAddress,
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as List<CartItem>;
                //Nếu giỏ hàng rỗng sẽ điều hướng tới trang Home
                if (cart == null || !cart.Any()) { return RedirectToAction("Index", "Home"); } //Nếu người dùng chưa đăng nhập sẽ điều hướng tới trang Login
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) { return RedirectToAction("Login", "Account"); }
                //nếu khách hàng không khớp với tên tên đăng nhập, sẽ điều hướng tới trang Login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }
                //Nếu người dùng chọn thanh toán bằng Paypal, sẽ điều hướng tới trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {

                    return RedirectToAction("PaymentWithPaypal", "PayPal", model);
                }

                // Thiết Lập PaymentStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chua thanh toán"; break;
                }



                //Tạo đơn hàng và ti tiết đơn hàng liên quan
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                //Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();
                // Xóa giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                // Điều hướng tới trang Xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }
    }
}



