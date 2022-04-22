using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FtxApi;
using FtxApi.Enums;
using FtxApi.Model;
using Newtonsoft.Json;

namespace FtxApi_Test
{
    class Program
    {
        static void Main()
        {
            var client = new Client("PWWK67zUR_wz9RkY62Cu6rwfq4i3RjBwjnbfsyR1", "N4heQxCnxD-ZpdErU-cGT7r4QLv6un15YbHvkFLz");
            var api = new FtxRestApi(client);
            var wsApi = new FtxWebSocketApi("wss://ftx.com/ws/");

            try
            {
                BuyAndSell(api).Wait();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BuyAndSell(api).Wait();
            }


            //RestTests(api).Wait();
            //WebSocketTests(wsApi, client).Wait();

            Console.ReadLine();
        }

        #region RestTests
        private static async Task RestTests(FtxRestApi api)
        {
            var ins = "APE/USD";

            var dateStart = DateTime.UtcNow.AddMinutes(-100);
            var dateEnd = DateTime.UtcNow.AddMinutes(-10);

            var r1 = api.GetCoinsAsync().Result;
            var r2 = api.GetAllFuturesAsync().Result;
            var r3 = api.GetFutureAsync(ins).Result;
            var r4 = api.GetFutureStatsAsync(ins).Result;
            var r5 = api.GetFundingRatesAsync(dateStart, dateEnd).Result;
            var r6 = api.GetHistoricalPricesAsync(ins, 300, 30, dateStart, dateEnd).Result;
            var r7 = api.GetMarketsAsync().Result;
            var r8 = api.GetSingleMarketsAsync(ins).Result;
            var r9 = api.GetMarketOrderBookAsync(ins, 20).Result;
            var r10 = api.GetMarketTradesAsync(ins, 20, dateStart, dateEnd).Result;
            var r11 = api.GetAccountInfoAsync().Result;
            var r12 = api.GetPositionsAsync().Result; 
            var r13 = api.ChangeAccountLeverageAsync(20).Result;
            var r14 = api.GetCoinAsync().Result;
            var r15 = api.GetBalancesAsync().Result;
            var r16 = api.GetDepositAddressAsync("BTC").Result;
            var r17 = api.GetDepositHistoryAsync().Result;
            var r18 = api.GetWithdrawalHistoryAsync().Result;
            var r19 = api.RequestWithdrawalAsync("USDTBEAR", 20.2m, "0x83a127952d266A6eA306c40Ac62A4a70668FE3BE", "", "", "").Result;
            var r21 = api.GetOpenOrdersAsync(ins).Result;
            var r20 = api.PlaceOrderAsync(ins, SideType.buy, 1000, OrderType.limit, 0.001, false).Result;
            var r20_1 = api.PlaceStopOrderAsync(ins, SideType.buy, 1000, 0.001m, false).Result;
            var r20_2 = api.PlaceTrailingStopOrderAsync(ins, SideType.buy, 0.05m, 0.001m, false).Result;
            var r20_3 = api.PlaceTakeProfitOrderAsync(ins, SideType.buy, 1000, 0.001m, false).Result;
            var r23 = api.GetOrderStatusAsync("12345").Result;
            var r24 = api.GetOrderStatusByClientIdAsync("12345").Result;
            var r25 = api.CancelOrderAsync("1234").Result;
            var r26 = api.CancelOrderByClientIdAsync("12345").Result;
            var r27 = api.CancelAllOrdersAsync(ins).Result;
            var r28 = api.GetFillsAsync(ins, 20, dateStart, dateEnd).Result;
            var r29 = api.GetFundingPaymentAsync(dateStart, dateEnd).Result;
            var r30 = api.GetLeveragedTokensListAsync().Result;
            var r31 = api.GetTokenInfoAsync("HEDGE").Result;
            var r32 = api.GetLeveragedTokenBalancesAsync().Result;
            var r33 = api.GetLeveragedTokenCreationListAsync().Result;
            var r34 = api.RequestLeveragedTokenCreationAsync("HEDGE", 100).Result;
            var r35 = api.GetLeveragedTokenRedemptionListAsync().Result;
            var r36 = api.RequestLeveragedTokenRedemptionAsync("HEDGE", 100).Result;
        }
        #endregion

        #region WebSocketTests
        private static async Task WebSocketTests(FtxWebSocketApi wsApi, Client client)
        {
            var ins = "APE/USD";
            
            wsApi.OnWebSocketConnect += () =>
            {
                wsApi.SendCommand(FtxWebSockerRequestGenerator.GetAuthRequest(client));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orderbook", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("trades", ins));
                wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("ticker", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("fills"));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orders"));
            };

            await wsApi.Connect();
        }
        #endregion

        #region BuyAndSell
        private static async Task BuyAndSell(FtxRestApi api)
        {
            // your future
            var ins = "APE/USD";

            #region declare            
            double? firstBalance = 0;
            double? askPrice = 0;
            double? askPrice1 = 0;
            double? askPrice2 = 0;
            double? askPrice3 = 0;
            double? askPrice4 = 0;
            double? askPrice5 = 0;
            double? askPrice6 = 0;
            double? askPrice7 = 0;
            double? askPrice8 = 0;
            double? askPrice9 = 0;
            double? askPrice10 = 0;
            double? buyPrice = 0;
            double? buyingPrice = 0;
            double? askPrice_sell = 0;
            string OrderID = string.Empty;
            double? profit = 0;
            double? sellProfit = 0;
            double? totalProfit = 0;
            int buyTimes = 0;
            OrderResult OrderResult = new OrderResult();
            #endregion

            while (true)
            {
                buyTimes++;

                #region get Balance
                var getBalance1 = api.GetBalancesAsync().Result;
                BalanceResult BalanceResult1 = JsonConvert.DeserializeObject<BalanceResult>(getBalance1);
                var BalanceList1 = BalanceResult1.result;
                foreach (var item in BalanceList1)
                {
                    if (item.coin == "USD")
                    {
                        Console.WriteLine("Round: " + buyTimes);
                        Console.WriteLine("Coin: " + item.coin + ", UsdValue: " + item.usdValue);
                        Console.WriteLine("Profit: " + (item.usdValue - firstBalance));
                        sellProfit = item.usdValue - firstBalance;
                        totalProfit = totalProfit + sellProfit;
                        Console.WriteLine("Total Profit: " + totalProfit);
                        Console.WriteLine("###########################################");
                        firstBalance = item.usdValue;
                    }
                }
                #endregion

                #region Buy
                var i = 1;
                bool isOrdering = false;
                bool isBought = false;
                while (!isBought)
                {
                    #region Get price 10 times
                    // Get first price
                    var buyMKPrice1 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy1 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice1);
                    var Market_Buy1 = MarketResult_Buy1.result;
                    askPrice1 = Market_Buy1.ask ?? 0;
                    // Get second price
                    var buyMKPrice2 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy2 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice2);
                    var Market_Buy2 = MarketResult_Buy2.result;
                    askPrice2 = Market_Buy2.ask ?? 0;
                    // Get third price
                    var buyMKPrice3 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy3 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice3);
                    var Market_Buy3 = MarketResult_Buy3.result;
                    askPrice3 = Market_Buy3.ask ?? 0;
                    // Get fourth price
                    var buyMKPrice4 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy4 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice4);
                    var Market_Buy4 = MarketResult_Buy4.result;
                    askPrice3 = Market_Buy4.ask ?? 0;
                    // Get fifth price
                    var buyMKPrice5 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy5 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice5);
                    var Market_Buy5 = MarketResult_Buy5.result;
                    askPrice5 = Market_Buy5.ask ?? 0;
                    // Get sixth price
                    var buyMKPrice6 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy6 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice6);
                    var Market_Buy6 = MarketResult_Buy6.result;
                    askPrice6 = Market_Buy3.ask ?? 0;
                    // Get seventh price
                    var buyMKPrice7 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy7 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice7);
                    var Market_Buy7 = MarketResult_Buy7.result;
                    askPrice7 = Market_Buy7.ask ?? 0;
                    // Get eighth price
                    var buyMKPrice8 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy8 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice8);
                    var Market_Buy8 = MarketResult_Buy8.result;
                    askPrice8 = Market_Buy8.ask ?? 0;
                    // Get ninth price
                    var buyMKPrice9 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy9 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice9);
                    var Market_Buy9 = MarketResult_Buy9.result;
                    askPrice9 = Market_Buy9.ask ?? 0;
                    // Get tenth price
                    var buyMKPrice10 = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy10 = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice10);
                    var Market_Buy10 = MarketResult_Buy10.result;
                    askPrice10 = Market_Buy10.ask ?? 0;
                    #endregion

                    #region Choose price condition
                    // If  first price <= 9th price && first price <= 10th price => choose first price
                    if (askPrice1 <= askPrice9 && askPrice1 <= askPrice10)
                        askPrice = askPrice1;
                    else if (askPrice1 - askPrice10 > 0.1)
                    {
                        continue;
                    }
                    // If  first price >= 9th price && first price >= 10th price=> choose 10th price
                    else if (askPrice1 >= askPrice9 && askPrice1 >= askPrice10)
                        askPrice = askPrice10;
                    else
                    {
                        var buyMKPrice = api.GetSingleMarketsAsync(ins).Result;
                        MarketResult MarketResult_Buy = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice);
                        var Market_Buy = MarketResult_Buy.result;
                        askPrice = Market_Buy.ask ?? 0;
                    }
                    #endregion

                    buyPrice = (askPrice  - 0.02);

                    var getApeBalance = api.GetBalancesAsync().Result;
                    BalanceResult ApeBalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getApeBalance);
                    var ApeBalanceList = ApeBalanceResult.result;

                    foreach (var item in ApeBalanceList)
                    {
                        if (item.coin == "APE")
                        {
                            if (item.total < 1)
                            {
                                // Buy Condition
                                if (!isOrdering)
                                {
                                    if (askPrice > 0)
                                    {
                                        // Input amount coin you want to buy
                                        var rBuy = api.PlaceOrderAsync(ins, SideType.buy, buyPrice ?? 0, OrderType.limit, 100, false).Result;
                                        buyingPrice = buyPrice;
                                        isOrdering = true;
                                        OrderResult = JsonConvert.DeserializeObject<OrderResult>(rBuy);
                                        if (i == 1) { OrderID = OrderResult.result.id; }
                                    }
                                }

                                if (i == 1)
                                    Console.WriteLine("Buying price:" + buyingPrice + ", waiting for buying...");
                                else if (i == 25)
                                {
                                    var cancel = api.CancelOrderAsync(OrderID);
                                    isOrdering = false;
                                    Console.WriteLine("Order canceled!");
                                    i = 0;
                                }
                                i++;
                            }
                            else if (item.total >= 1)
                            {
                                Console.WriteLine("Buy Price: " + buyPrice);
                                Console.WriteLine("Buy Success!");
                                Console.WriteLine("###########################################");
                                isBought = true;
                                i = 1;
                            }
                        }
                    }
                }
                #endregion

                #region Sell
                bool isSelling = false;
                while (isBought)
                {
                    // Sell Condition
                    var getApeBalance = api.GetBalancesAsync().Result;
                    BalanceResult ApeBalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getApeBalance);
                    var ApeBalanceList = ApeBalanceResult.result;
                    foreach (var item in ApeBalanceList)
                    {
                        if (item.coin == "APE")
                        {
                            if (item.total >= 1) 
                            {
                                if (!isSelling)
                                {
                                    //var sellMKPrice = api.GetSingleMarketsAsync(ins).Result;
                                    //MarketResult MarketResult_Sell = JsonConvert.DeserializeObject<MarketResult>(sellMKPrice);
                                    //var Market_Sell = MarketResult_Sell.result;
                                    //askPrice_sell = Market_Sell.ask ?? 0;
                                    askPrice_sell = askPrice + 0.01;
                                    profit = ((askPrice_sell) - (buyPrice));

                                    var rSell = api.PlaceOrderAsync(ins, SideType.sell, askPrice_sell ?? 0, OrderType.limit, item.total ?? 0, false).Result;
                                    isSelling = true;
                                    Console.WriteLine(rSell);
                                    Console.WriteLine("Profit: " + profit);
                                    Console.WriteLine("Sell Price: " + askPrice_sell);
                                    Console.WriteLine("Waiting for selling...");
                                }
                            }
                            else if (item.total < 1)
                            { 
                                Console.WriteLine("Sell Success!");
                                Console.WriteLine("###########################################");
                                isSelling = false;
                                isBought = false;
                            }
                        }
                    }
                }
                #endregion
                
            }
        }
        #endregion

        


    }
}
