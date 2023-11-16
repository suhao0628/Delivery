using Delivery_API.Models.Entity;
using Delivery_API.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace Delivery_API.Data
{
    public class Initializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                context.Database.Migrate();

                var dishes = new List<Dish> {
                   new Dish
                   {
                       Id = Guid.NewGuid(),
                       Name = "4 сыра",
                       Description = "4 сыра: «Моцарелла», «Гауда», «Фета», «Дор-блю», сливочно-сырный соус, пряные травы",
                       Price = 360,
                       Image = "https://mistertako.ru/uploads/products/77888c7e-8327-11ec-8575-0050569dbef0.",
                       Vegetarian = true,
                       Category = DishCategory.Pizza,
                       Rating = 6.591836734693878,
                   },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Party BBQ",
                        Description = "Бекон, соленый огурчик, брусника, сыр «Моцарелла», сыр «Гауда», соус BBQ",
                        Price = 480,
                        Image = "https://mistertako.ru/uploads/products/839d0250-8327-11ec-8575-0050569dbef0.",
                        Vegetarian = false,
                        Category = DishCategory.Pizza,
                        Rating = 5.173469387755103,
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wok а-ля Диаблo",
                        Description =
                        "Пшеничная лапша обжаренная на воке с колбасками пепперони, маслинами, сладким перцем и перцем халапеньо в томатном соусе с добавлением петрушки. БЖУ на 100 г. Белки, г — 8,18 Жиры, г — 16,33 Углеводы, г — 20,62",
                        Price = 330,
                        Image = "https://mistertako.ru/uploads/products/663ab868-85ec-11ea-a9ab-86b1f8341741.jpg",
                        Vegetarian = false,
                         Category = DishCategory.Wok,
                         Rating =3.7142857142857144,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wok болоньезе",
                        Description =
                        "Пшеничная лапша обжаренная на воке с фаршем (Говядина/свинина) и овощами (шампиньоны, перец сладкий, лук красный) в томатном соусе с добавлением чесночно–имбирной заправки и петрушки. БЖУ на 100 г. Белки, г — 8,07 Жиры, г — 15,38 Углеводы, г — 23,22",
                        Price = 290,
                        Image = "https://mistertako.ru/uploads/products/663ab866-85ec-11ea-a9ab-86b1f8341741.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Wok,
                         Rating =9,


                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wok том ям с курицей",
                        Description =
                        "Лапша пшеничная, куриное филе, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 7,05 Жиры, г - 12,92 Углеводы, г - 18,65",
                        Price = 280,
                        Image = "https://mistertako.ru/uploads/products/a41bd9fd-54ed-11ed-8575-0050569dbef0.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Wok,
                        Rating =9,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wok том ям с морепродуктам",
                        Description =
                        "Лапша пшеничная, креветки, кальмар, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 8,57 Жиры, г - 12,8 Углеводы, г - 18,8",
                        Price = 340,
                        Image = "https://mistertako.ru/uploads/products/bacd88ca-54ed-11ed-8575-0050569dbef0.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Wok,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wok том ям с овощами",
                        Description = "Лапша пшеничная, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 5,32 Жиры, г - 14,89 Углеводы, г - 22,46",
                        Price = 250,
                        Image = "https://mistertako.ru/uploads/products/cd661716-54ed-11ed-8575-0050569dbef0.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Wok,
                        Rating =5,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Белиссимо",
                        Description = "Копченая куриная грудка, свежие шампиньоны, маринованные опята, сыр «Моцарелла», сыр «Гауда», сливочно-чесночный соус, свежая зелень.",
                        Price = 400,
                        Image = "https://mistertako.ru/uploads/products/9ee8ed49-8327-11ec-8575-0050569dbef0.",
                        Vegetarian = false,
                        Category = DishCategory.Pizza,
                        Rating=7.25,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Коктейль классический",
                        Description = "Классический молочный коктейль",
                        Price = 140,
                        Image = "https://mistertako.ru/uploads/products/120b46bc-5f32-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Drink,
                        Rating=5.89599609375,
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Коктейль клубничный",
                        Description = "Классический молочный коктейль с клубничным топпингом",
                        Price = 170,
                        Image = "https://mistertako.ru/uploads/products/120b46bd-5f32-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Drink,
                        Rating=6,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Коктейль шоколадный",
                        Description = "Классический молочный коктейль с добавлением шоколадного топпинга",
                        Price = 170,
                        Image = "https://mistertako.ru/uploads/products/120b46be-5f32-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Drink
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Морс cмородиновый",
                        Description = "Смородиновый морс",
                        Price = 90,
                        Image = "https://mistertako.ru/uploads/products/120b46c1-5f32-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Drink
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Морс облепиховый",
                        Description = "Облепиха, имбирь, сахар",
                        Price = 90,
                        Image ="https://mistertako.ru/uploads/products/5a7d58a5-879d-11eb-850a-0050569dbef0.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Drink
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Рамен сырный",
                        Description = "Сырный бульон с пшеничной лапшой, отварным куриным филе,помидором и сырными шариками. БЖУ на 100 г. Белки, г — 11,8 Жиры, г — 9,82 Углеводы, г — 22,69",
                        Price = 300,
                        Image = "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Soup,
                        Rating=5,
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name ="Сладкий ролл с апельсином и бананом",
                        Description = "Апельсин, банан, шоколадная крошка, сыр творожный, сырная лепешка. БЖУ на 100 г. Белки, г - 5,86 Жиры, г - 13,12 Углеводы, г - 44,05",
                        Price = 250,
                        Image = "https://mistertako.ru/uploads/products/05391255-54ee-11ed-8575-0050569dbef0.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Dessert,
                        Rating=5,
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Сладкий ролл с арахисом и бананом",
                        Description = "Сырная лепешка, банан, арахис, сливочный сыр, шоколадная крошка, топинг карамельный",
                        Price = 210,
                        Image = "https://mistertako.ru/uploads/products/a4772f7a-7a6f-11eb-850a-0050569dbef0.jpeg",
                        Vegetarian = true,
                        Category = DishCategory.Dessert,
                        Rating=7,
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Сладкий ролл с бананом и киви",
                        Description = "Сырная лепешка, банан, киви, сливочный сыр, топинг клубничный",
                        Price = 220,
                        Image = "https://mistertako.ru/uploads/products/9e7c8581-7a6f-11eb-850a-0050569dbef0.jpeg",
                        Vegetarian = true,
                        Category = DishCategory.Dessert
                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Сливочный рамен с курицей и шампиньонами",
                        Description =
                        "Бульон рамен со сливками (куриный бульон, чесночно-имбирная заправка, соевый соус) с пшеничной лапшой, отварной курицей, омлетом Томаго и шампиньонами. БЖУ на 100 г. Белки, г — 8,13 Жиры, г — 6,18 Углеводы, г — 8,08",
                        Price = 260,
                        Image = "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Soup,
                        Rating=3,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Том ям кай",
                        Description = "Знаменитый тайский острый суп со сливками, куриным филе, шампиньонами, красным луком, помидором, перчиком Чили и кинзой. Подается с рисом. БЖУ на 100 г. Белки, г — 5,75 Жиры, г — 3,72 Углеводы, г — 14,76",
                        Price = 300,
                        Image = "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = false,
                        Category = DishCategory.Soup,
                        Rating=10,

                    },
                    new Dish
                    {
                        Id = Guid.NewGuid(),
                        Name = "Чизкейк Нью-Йорк",
                        Description = "Чизкейк Нью-Йорк - настоящая классика. Его основа - сочетание вкусов нежнейшего сливочного сыра и тонкой песочной корочки.",
                        Price = 210,
                        Image = "https://mistertako.ru/uploads/products/120b46b1-5f32-11e8-8f7d-00155dd9fd01.jpg",
                        Vegetarian = true,
                        Category = DishCategory.Dessert

                    },
                };

                if (!context.Dishes.Any())
                {
                    context.AddRange(dishes);
                    context.SaveChanges();
                }

            }
        }
    }
}
