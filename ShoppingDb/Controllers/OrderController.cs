using Iyzipay;
using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using ShoppingApi.extensions;
using System.Security.Claims;
using Address = Iyzipay.Model.Address;
using Iyzipay.Request;
using Newtonsoft.Json;
using System.Globalization;
namespace ShoppingApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _config;
        public OrderController(DataContext context, IConfiguration config)
        {

            _context = context;
            _config = config;
        }
        [HttpGet("getorders")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            return await _context.Orders.Include(x => x.OrderItems)
         .OrderToDTO()
                .Where(x => x.UserId == userId).OrderByDescending(x => x.OrderDate)
                .ToListAsync(); ;
        }



        [HttpGet("{id}", Name = "getorder")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrder(int id)
        {
            return await _context.Orders.Include(x => x.OrderItems)
                .OrderToDTO()
                .Where(x => x.UserId == User.Identity!.Name && x.Id == id).ToListAsync(); ;
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO createOrderDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var cart = await _context.Carts.Include(i => i.CartItems)
            .ThenInclude(i => i.Product)
            .Where(i => i.userId == userId).FirstOrDefaultAsync();

            if (cart == null || cart.CartItems.Count == 0)
            {
                return BadRequest("Your cart is empty");
            }
            var items = new List<Entity.OrderItem>();

            foreach (var item in cart.CartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                var orderItem = new Entity.OrderItem
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    Name = product.Name,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);

            }
            var subTotal = items.Sum(i => i.Price * i.Quantity);

            var orderAddress = await _context.Addresses.Where(i => i.Id == createOrderDTO.AdressId).FirstOrDefaultAsync();

            var order = new Order
            {
                OrderItems = items,
                UserId = userId,
                Phone = orderAddress.Phone,
                SubTotal = subTotal,
                ApartmanNo = orderAddress.ApartmanNo,
                Cadde = orderAddress.Cadde,
                DaireNo = orderAddress.DaireNo,
                FullAddress = orderAddress.FullAddress,
                Sehir = orderAddress.Sehir,
                Sokak = orderAddress.Sokak,
                Ilce = orderAddress.Ilce,

            };
           //payment buyer
           var paymentBuyerName= await _context.Users.Where(i=>i.Id==order.UserId).Select(i => i.Name).FirstOrDefaultAsync();
           var paymentBuyerSurName= await _context.Users.Where(i=>i.Id==order.UserId).Select(i => i.SurName).FirstOrDefaultAsync();
            //email buyer 
            var emailBuyer = await _context.Users.Where(i => i.Id == order.UserId).Select(i => i.Email).FirstOrDefaultAsync();

            var card = createOrderDTO.Card;
            string formattedTotal = subTotal.ToString("0.00", CultureInfo.InvariantCulture);
         var paymentResult=   await ProcessPayment(card, formattedTotal, order.Id.ToString(),orderAddress,paymentBuyerName,paymentBuyerSurName,emailBuyer,createOrderDTO );


            if (paymentResult.Status != "success")
            {
                switch (paymentResult.ErrorCode)
                {
                    case "10051":
                        return BadRequest(new { error = "Kart limiti yetersiz" });
                    case "10005":
                        return BadRequest(new { error = "İşlem onaylanmadı" });
                    case "10012":
                        return BadRequest(new { error = "Geçersiz işlem" });
                    case "6001":
                        return BadRequest(new { error = "Kayıp ya da çalıntı kart" });
                    case "10054":
                        return BadRequest(new { error = "Son kullanma tarihi hatalı" });
                    case "10084":
                        return BadRequest(new { error = "CVV bilgisi hatalı" });
                    case "10057":
                        return BadRequest(new { error = "Bu kartla işlem yapılamıyor. Lütfen farklı bir kart deneyin veya bankanızla iletişime geçin." });
                    case "10058":
                        return BadRequest(new { error = "Bu işlem kartınız tarafından desteklenmiyor. Lütfen farklı bir kart deneyin." });
                    case "10034":
                        return BadRequest(new { error = "Güvenlik nedeniyle işlem reddedildi. Lütfen bankanızla iletişime geçin." });
                    case "10093":
                        return BadRequest(new { error = "Kartınız internetten alışverişe kapalıdır. Açtırmak için bankanız ile irtibata geçebilirsiniz." });
                    case "10202":
                        return BadRequest(new { error = "Ödeme işlemi esnasında genel bir hata oluştu" });
                    case "1":
                        return BadRequest(new { error = "Sistem hatası oluştu" });
                    case "13":
                        return BadRequest(new { error = "Kart tarih bilgisi eşleşmiyor" });
                    default:
                        return BadRequest(new { error = "Bilinmeyen bir hata oluştu: " });
                }
            }
            else
            {
                _context.Orders.Add(order);
                _context.Carts.Remove(cart);
                _context.SaveChanges();
                order.OrderStatus = OrderStatus.Approved;

                return CreatedAtRoute(nameof(GetOrder), new { id = order.Id }, order.Id);
            }
        }


        [HttpGet("installment-options/{bin}/{price}")]
        public async Task<ActionResult> CartControl(string bin, string price)
        {
            var request = new RetrieveInstallmentInfoRequest
            {
                Locale = "tr",
                BinNumber = bin,
                ConversationId = Guid.NewGuid().ToString(),
                Price = price
            };
            var options = new Options
            {
                ApiKey = _config["PaymentAPI:APIKey"],
                SecretKey = _config["PaymentAPI:SecretKey"],
                BaseUrl = "https://sandbox-api.iyzipay.com"
            };
            var result= await InstallmentInfo.Retrieve(request, options);
            return Ok(result);
        }

        private async Task<Payment> ProcessPayment(CardDTO card, string price, string orderId,Entity.Address address,string paymentBuyerName,string paymentBuyerSurName, string emailBuyer ,CreateOrderDTO createOrderDTO)
        {

      

            Options options = new Options();
            options.ApiKey = _config["PaymentAPI:APIKey"];
            options.SecretKey = _config["PaymentAPI:SecretKey"];
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            var installmentRequest = new RetrieveInstallmentInfoRequest
            {
                Locale = "tr",
                BinNumber = card.CardNumber.Substring(0, 6),
                ConversationId = Guid.NewGuid().ToString(),
                Price = price
            };


            var installmentResult = await InstallmentInfo.Retrieve(installmentRequest, options);

            var selectedInstallment = installmentResult.InstallmentDetails
                .FirstOrDefault()?.InstallmentPrices
                .FirstOrDefault(i => i.InstallmentNumber == card.Installment);
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.Price = price;
            request.PaidPrice = selectedInstallment.TotalPrice.ToString(CultureInfo.InvariantCulture); ;
            request.Currency = Currency.TRY.ToString();
            request.Installment = card.Installment;
            request.BasketId = orderId;
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = card.CardHolderName;
            paymentCard.CardNumber = card.CardNumber;
            paymentCard.ExpireMonth = card.ExpireMonth;
            paymentCard.ExpireYear = 20+card.ExpireYear;
            paymentCard.Cvc = card.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = address.UserId;
            buyer.Name = paymentBuyerName;
            buyer.Surname = paymentBuyerSurName;
            buyer.GsmNumber = address.Phone;
            buyer.Email = emailBuyer;
            buyer.RegistrationAddress = address.FullAddress;
            buyer.City = address.Sehir;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            buyer.IdentityNumber = "12345678901";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = address.AdSoyad;
            shippingAddress.City = address.Sehir;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = address.FullAddress;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = address.AdSoyad;
            billingAddress.City = address.Sehir;
            billingAddress.Country = "Turkey";
            billingAddress.Description = address.FullAddress;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            
            foreach (var orderItem in createOrderDTO.OrderItems)
            {
                var basketItem = new BasketItem
                {
                    Id = orderItem.ProductId.ToString(),
                    Name = orderItem.Name,
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (orderItem.Price * orderItem.Quantity).ToString("0.00", CultureInfo.InvariantCulture),
                    Category1 = string.Join(", ", GetProductCategories(orderItem.ProductId)),
                   

                };
                basketItems.Add(basketItem);
            }
            request.BasketItems = basketItems;

            var result = await Payment.Create(request, options);
            Console.WriteLine(result.Status == "success" ? "Ödeme başarılı!" : $"Ödeme başarısız: {result.ErrorMessage}");
            return result;
        }


        private List<string> GetProductCategories(int productId)
        {
            var productCategories = _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Where(p => p.Id == productId)
                .ToList();

            return productCategories
                .SelectMany(p => p.ProductCategories)
                .Select(pc => pc.Category.Name)
                .ToList();
        }
     


    }
}
