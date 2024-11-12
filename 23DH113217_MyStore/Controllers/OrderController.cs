using _23DH113217_MyStore.Models.ViewModel;
using _23DH113217_MyStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

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
        
        
        public ActionResult Checkout()
        {
            //Kiểm tra giỏ hàng trong session,
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            var username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                // Nếu không có username trong session, chuyển hướng người dùng tới trang đăng nhập
                return RedirectToAction("Login", "Account");
            }


            // Lấy tên người dùng từ User.Identity
            var user = db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tìm khách hàng trong bảng Customers dựa vào CustomerID
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Register", "Account"); // Hoặc xử lý lỗi khác​14:04/-strong/-heart:>:o:-((:-h Xem trước khi gửiThả Files vào đây để xem lại trước khi gửi
            }
                var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                TotalAmount = cart.Items.Sum(item => item.TotalPrice),
                OrderDate = DateTime.Now,
                ShippingAddress = customer.CustomerAddress,
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as Cart;
                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("Index", "Home");
                }
                var username = Session["Username"] as string;
                if (string.IsNullOrEmpty(username))
                {
                    // Nếu không có username trong session, chuyển hướng người dùng tới trang đăng nhập
                    return RedirectToAction("Login", "Account");
                }


                // Lấy tên người dùng từ User.Identity
                var user = db.Users.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Tìm khách hàng trong bảng Customers dựa vào CustomerID
                var customer = db.Customers.SingleOrDefault(c => c.Username == username);
                if (customer == null)
                {
                    return RedirectToAction("Register", "Account"); // Hoặc xử lý lỗi khác​14:04/-strong/-heart:>:o:-((:-h Xem trước khi gửiThả Files vào đây để xem lại trước khi gửi
                }
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
                    OrderDetails = cart.Items.Select(item => new OrderDetail
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



