﻿using System;
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
            catch (Exception ex) { 
            
            }
            finally {
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
            var r20 = api.PlaceOrderAsync(ins, SideType.buy, 1000, OrderType.limit, 0.001m, false).Result;
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
                wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orderbook", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("trades", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("ticker", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("fills"));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orders"));
            };

            await wsApi.Connect();
        }
        #endregion

        #region BuyAndSell
        private static async Task BuyAndSell(FtxRestApi api)
        {
            var ins = "APE/USD";
            bool isBought = false;
            decimal bidPrice = 0;
            decimal buyPrice = 0;
            decimal sellPrice = 0;
            decimal bidPrice_sell = 0;
            string OrderID = string.Empty;
            decimal profit = 0;
            decimal totalProfit = 0;

            var getBalance = api.GetBalancesAsync().Result;
            BalanceResult BalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getBalance);
            var BalanceList = BalanceResult.result;
            decimal? usdValue = 0;
            foreach (var item in BalanceList)
            {
                if (item.coin == "USD")
                {
                    usdValue = item.usdValue;
                    Console.WriteLine("Coin: " + item.coin + ", UsdValue: " + item.usdValue + ", Total: " + item.total);
                    Console.WriteLine("###########################################");
                }
            }

            while (true)
            {
                var i = 1;

                #region Market Price (Buy)
                while (!isBought)
                {
                    var buyMKPrice = api.GetSingleMarketsAsync(ins).Result;
                    MarketResult MarketResult_Buy = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice);
                    var Market_Buy = MarketResult_Buy.result;
                    bidPrice = Market_Buy.bid ?? 0;
                    buyPrice = ((bidPrice * 100) - 2) / 100;
                    
                    // Buy Condition
                    var rBuy = api.PlaceOrderAsync(ins, SideType.buy, buyPrice, OrderType.limit, 100, false).Result;
                    OrderResult OrderResult = JsonConvert.DeserializeObject<OrderResult>(rBuy);
                    if(i == 1) { OrderID = OrderResult.result.id; }
                    

                    var getApeBalance = api.GetBalancesAsync().Result;
                    BalanceResult ApeBalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getApeBalance);
                    var ApeBalanceList = ApeBalanceResult.result;

                    foreach (var item in ApeBalanceList)
                    {
                        if (item.coin == "APE")
                        {
                            if (item.total < 1)
                            {
                                if (i == 25)
                                {
                                    var cancel = api.CancelOrderAsync(OrderID);
                                    Console.WriteLine("Order canceled!");
                                    i = 1;
                                }
                                Console.WriteLine("Waiting for buying..." + i);
                                i++;
                            }
                            else if (item.total >= 1)
                            {
                                Console.WriteLine("Buy Price: " + buyPrice);
                                Console.WriteLine("Buy Success!");
                                Console.WriteLine("###########################################");
                                isBought = true;
                            }
                        }
                    }
                }
                #endregion

                #region Sell
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
                                var sellMKPrice = api.GetSingleMarketsAsync(ins).Result;
                                MarketResult MarketResult_Sell = JsonConvert.DeserializeObject<MarketResult>(sellMKPrice);
                                var Market_Sell = MarketResult_Sell.result;
                                bidPrice_sell = Market_Sell.bid ?? 0;
                                profit = ((bidPrice_sell * 100) - (buyPrice * 100));
                                //sellPrice = ((buyPrice * 100) + 2) / 100;
                                Console.WriteLine("Waiting for selling...");
                                Console.WriteLine("Profit: " + profit);
                                if ((bidPrice_sell * 100) - (buyPrice * 100) >= 2)
                                {
                                    var rSell = api.PlaceOrderAsync(ins, SideType.sell, bidPrice_sell, OrderType.limit, item.total ?? 0, false).Result;
                                    Console.WriteLine(rSell);
                                    Console.WriteLine("Sell Price: " + sellPrice);
                                }
                                else if ((bidPrice_sell * 100) - (buyPrice * 100) < -15)
                                {
                                    var rSell = api.PlaceOrderAsync(ins, SideType.sell, bidPrice_sell, OrderType.limit, item.total ?? 0, false).Result;
                                    Console.WriteLine(rSell);
                                    Console.WriteLine("Sell Price: " + sellPrice);
                                }
                            }
                            else if (item.total < 1)
                            {
                                Console.WriteLine("Sell Success!");
                                Console.WriteLine("###########################################");
                                isBought = false;
                            }
                        }
                    }
                }
                #endregion

                #region get Balance
                var getBalance1 = api.GetBalancesAsync().Result;
                BalanceResult BalanceResult1 = JsonConvert.DeserializeObject<BalanceResult>(getBalance1);
                var BalanceList1 = BalanceResult1.result;
                foreach (var item in BalanceList1)
                {
                    if (item.coin == "USD")
                    {
                        Console.WriteLine("Coin: " + item.coin + ", UsdValue: " + item.usdValue + ", Total: " + item.total);
                        Console.WriteLine("Profit: " + (item.usdValue - usdValue));
                        totalProfit = totalProfit + profit;
                        Console.WriteLine("Total Profit: " + totalProfit);
                        Console.WriteLine("###########################################");
                        usdValue = item.usdValue;
                    }
                }
                #endregion
            }
        }
        #endregion

    }
}
