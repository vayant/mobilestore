using mobilestore.Models;

namespace mobilestore.Data
{
    public static class DbInitializer
    {
        public static void ResetAdminBalance(ApplicationDbContext context)
        {
            var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (admin != null)
            {
                admin.Balance = 0;
                context.SaveChanges();
            }
        }

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            
            if (!context.Users.Any())
            {
                var users = new UserClass[]
                {
                    new UserClass
                    {
                        Username = "admin",
                        Password = "123",
                        Email = "admin@mail.ru",
                        Role = "Admin"
                    },
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var products = new ProductClass[]
                {
                    new ProductClass
                    {
                        Name = "iPhone 15 Pro",
                        Price = 109999,
                        Description = "Флагманский смартфон с процессором A17 Pro и титановым корпусом",
                        ImageUrl = "/images/iphone-15-pro.jpg",
                        Category = "Apple",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Samsung Galaxy S24 Ultra",
                        Price = 129999,
                        Description = "Премиальный смартфон с поддержкой S Pen и мощной камерой",
                        ImageUrl = "/images/galaxy-s24-ultra.jpg",
                        Category = "Samsung",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Xiaomi 14 Pro",
                        Price = 89999,
                        Description = "Флагманский смартфон Xiaomi с камерой Leica",
                        ImageUrl = "/images/xiaomi-14-pro.jpg",
                        Category = "Xiaomi",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Huawei P60 Pro",
                        Price = 79999,
                        Description = "Премиальный смартфон с продвинутой камерой XMAGE",
                        ImageUrl = "/images/huawei-p60-pro.jpg",
                        Category = "Huawei",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "iPhone 15",
                        Price = 89999,
                        Description = "Новый iPhone 15 с динамическим островком",
                        ImageUrl = "/images/iphone-15.jpg",
                        Category = "Apple",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Samsung Galaxy S24+",
                        Price = 99999,
                        Description = "Мощный смартфон с поддержкой Galaxy AI",
                        ImageUrl = "/images/galaxy-s24-plus.jpg",
                        Category = "Samsung",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Xiaomi 14",
                        Price = 69999,
                        Description = "Компактный флагман с отличной камерой",
                        ImageUrl = "/images/xiaomi-14.jpg",
                        Category = "Xiaomi",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Huawei Mate 60 Pro",
                        Price = 89999,
                        Description = "Инновационный смартфон с продвинутой камерой",
                        ImageUrl = "/images/huawei-mate-60-pro.jpg",
                        Category = "Huawei",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "iPhone 15 Plus",
                        Price = 99999,
                        Description = "iPhone 15 Plus с увеличенным экраном",
                        ImageUrl = "/images/iphone-15-plus.jpg",
                        Category = "Apple",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Samsung Galaxy Z Fold5",
                        Price = 159999,
                        Description = "Инновационный складной смартфон",
                        ImageUrl = "/images/galaxy-z-fold5.jpg",
                        Category = "Samsung",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Xiaomi Redmi Note 13 Pro",
                        Price = 39999,
                        Description = "Мощный смартфон среднего класса с отличной камерой",
                        ImageUrl = "/images/redmi-note-13-pro.jpg",
                        Category = "Xiaomi",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Samsung Galaxy A54",
                        Price = 34999,
                        Description = "Популярный смартфон среднего класса с отличным экраном",
                        ImageUrl = "/images/galaxy-a54.jpg",
                        Category = "Samsung",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Bananaphone",
                        Price = 999999,
                        Description = "Последняя новинка моды от Apple с умопомрачительными характеристиками",
                        ImageUrl = "/images/banana.jpg",
                        Category = "Iphone",
                        InStock = true
                    },
                    new ProductClass
                    {
                        Name = "Xiaomi Oldphone 3000",
                        Price = 999,
                        Description = "Шикарный телефон от Xiaomi для самых утончённых личностей",
                        ImageUrl = "/images/Oldphone3000.png",
                        Category = "Xiaomi",
                        InStock = false
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
} 