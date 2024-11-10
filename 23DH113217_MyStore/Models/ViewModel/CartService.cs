using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Collections.Specialized.BitVector32;
using System.Web.SessionState;

namespace _23DH113217_MyStore.Models.ViewModel
{
    public class CartService
    {

        private readonly HttpSessionStateBase session;
        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        } 

        public Cart GetCart()
        {
            var cart = (Cart)session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                session["Cart"] = cart;
                    
            }
            return cart;
        }
        public void ClearCart()
        {
            session["Cart"] = null;
        }

        }
    }
