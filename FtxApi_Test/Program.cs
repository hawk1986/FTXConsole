using System;
using System.Collections.Generic;
using System.Net.Http;
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
            var client = new Client("lRBAa_NfGBBjjgT50rSBkgnx5UnGCSQCswpsjDUB", "FaHdaCiIQcyhP0opbSlnt6io3Ug7xz1tNvhwz9Wy");
            var api = new FtxRestApi(client);
            var wsApi = new FtxWebSocketApi("wss://ftx.com/ws/");

            try
            {
                //var price = GetHistoryPrices(api, "APE");

                BuyAndSell(api).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ", "+ ex.InnerException.Message);

                #region Cancel Orders
                // if error or exit, cancel orders...
                //var isCanceled = false;
                //var ins = "APE/USD";
                //while (!isCanceled)
                //{
                //    var checkOrders = api.GetOpenOrdersAsync(ins).Result;
                //    OrderStatus orderStatus = JsonConvert.DeserializeObject<OrderStatus>(checkOrders);
                //    var status = orderStatus.result;
                //    if (status.Count > 0)
                //    {
                //        var cancelAllOders = api.CancelAllOrdersAsync(ins).Result;
                //    }
                //    else
                //    {
                //        isCanceled = true;
                //    }
                //}

                //foreach (var item in status)
                //{
                //    if (item.market == "APE/USD" && item.side == "buy")
                //    {
                //        var cancel = api.CancelOrderAsync(item.id);
                //        Console.WriteLine("Order canceled!");
                //    }
                //    else if (item.market == "APE/USD" && item.side == "sell")
                //    {
                //        var cancel = api.CancelOrderAsync(item.id);
                //        Console.WriteLine("Order canceled!");
                //    }
                //}
                #endregion

                Main();

            }
            finally
            {
                #region Cancel Orders
                // if error or exit, cancel orders...
                //var isCanceled = false;
                //var ins = "APE/USD";
                //while (!isCanceled)
                //{
                //    var checkOrders = api.GetOpenOrdersAsync(ins).Result;
                //    OrderStatus orderStatus = JsonConvert.DeserializeObject<OrderStatus>(checkOrders);
                //    var status = orderStatus.result;
                //    if (status.Count > 0)
                //    {
                //        var cancelAllOders = api.CancelAllOrdersAsync(ins).Result;
                //    }
                //    else
                //    {
                //        isCanceled = true;
                //    }
                //}
                #endregion
            }

            //RestTests(api).Wait();

            //WebSocketTests(wsApi, client).Wait();

            Console.ReadLine();

        }

        #region BuyAndSell
        private static async Task BuyAndSell(FtxRestApi api)
        {
            // your future
            var ins = "APE/USD";

            #region declare            
            double? usdAvailableWithoutBorrow = 0;
            double? firstBalance = 0;
            string OrderID = string.Empty;
            double? sellProfit = 0;
            double? totalProfit = 0;
            int boughtTimes = 0;
            OrderResult OrderResult = new OrderResult();
            // buy param
            var i = 1;
            double _askPrice_buy = 0;
            bool isOrdering = false;
            bool isBought = false;
            bool canBuy = false;

            // sell param
            var j = 1;
            double _askPrice_sell = 0;
            bool isSelling = false;
            bool alreadySelling = false;
            #endregion

            while (true)
            {

                boughtTimes++;

                #region get Balance
                var getBalance1 = api.GetBalancesAsync().Result;
                BalanceResult BalanceResult1 = JsonConvert.DeserializeObject<BalanceResult>(getBalance1);
                var BalanceList1 = BalanceResult1.result;
                foreach (var item in BalanceList1)
                {
                    #region USD
                    if (item.coin == "USD")
                    {
                        var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
                        Console.WriteLine("NowTime: " + nowTime);
                        Console.WriteLine("Round: " + boughtTimes);
                        Console.WriteLine("Coin: " + item.coin + ", AvailableWithoutBorrow: " + item.availableWithoutBorrow);
                        usdAvailableWithoutBorrow = item.availableWithoutBorrow;
                        firstBalance = firstBalance == 0 ? usdAvailableWithoutBorrow : firstBalance;
                        Console.WriteLine("Profit: " + (usdAvailableWithoutBorrow - firstBalance));
                        sellProfit = usdAvailableWithoutBorrow - firstBalance;
                        totalProfit = totalProfit + sellProfit;
                        Console.WriteLine("Total Profit: " + totalProfit);
                        Console.WriteLine("###########################################");
                    }
                    #endregion

                    #region Ape Coin
                    //if (item.coin == "APE")
                    //{
                    //    Console.WriteLine("Round: " + buyTimes);
                    //    Console.WriteLine("Coin: " + item.coin + ", AvailableWithoutBorrow: " + item.availableWithoutBorrow);
                    //    firstBalance = firstBalance == 0 ? item.availableWithoutBorrow : firstBalance;
                    //    Console.WriteLine("Profit: " + (item.availableWithoutBorrow - firstBalance));
                    //    sellProfit = item.availableWithoutBorrow - firstBalance;
                    //    totalProfit = totalProfit + sellProfit;
                    //    Console.WriteLine("Total Profit: " + totalProfit);
                    //    Console.WriteLine("###########################################");
                    //    firstBalance = item.usdValue;
                    //}
                    #endregion
                }
                #endregion

                #region Buy
                while (!isBought)
                {
                    #region Get History Prices & decide the buying and selling price
                    if (!isOrdering)
                    {
                        if (DateTime.UtcNow.Second == 0 || DateTime.UtcNow.Second == 15 || DateTime.UtcNow.Second == 30 || DateTime.UtcNow.Second == 45)
                            canBuy = GetHistoryPrices(api, "APE");

                        // 2022.05.26
                        // open > close == red
                        // close > low == green
                    }
                    #endregion

                    if (canBuy)
                    {
                        #region Buying
                    //var getApeBalance = api.GetBalancesAsync().Result;
                    //BalanceResult ApeBalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getApeBalance);
                    //var ApeBalanceList = ApeBalanceResult.result;

                    //foreach (var item in ApeBalanceList)
                    //{
                    //    if (item.coin == "APE")
                    //    {
                    //        if (item.availableWithoutBorrow < 5)
                    //        {
                    //            // Buy Condition
                    //            if (!isOrdering)
                    //            {
                    //                if (wantBuyPrice >= 0 && _askPrice_buy <= wantBuyPrice)
                    //                    buyPrice = (_askPrice_buy);

                    //                // check if enough balance
                    //                var canBuy = (usdAvailableWithoutBorrow ?? 0) / _askPrice_buy;

                    //                if (buyPrice > 0)
                    //                {
                    //                    // Input amount coin you want to buy
                    //                    if (canBuy - 2 > 1)
                    //                    {
                    //                        //var rBuy = api.PlaceOrderAsync(ins, SideType.buy, (buyPrice ?? 0), OrderType.limit, canBuy - 2, false).Result;
                    //                        buyingPrice = buyPrice;
                    //                        isOrdering = true;
                    //                        i = 1;
                    //                        //OrderResult = JsonConvert.DeserializeObject<OrderResult>(rBuy);
                    //                        //if (i == 1) { OrderID = OrderResult.result.id; }
                    //                        #region need to sheck if really bought (using while)
                    //                        #endregion
                    //                    }
                    //                }
                    //            }

                    //            if (i == 1 && isOrdering)
                    //                Console.WriteLine("Buying price:" + buyingPrice + ", waiting for buying...");
                    //            //else if (i == 70)
                    //            //{
                    //            //    var cancel = api.CancelOrderAsync(OrderID);
                    //            //    isOrdering = false;
                    //            //    Console.WriteLine("Order canceled!");
                    //            //    i = 0;
                    //            //}
                    //            i++;
                    //        }
                    //        else if (item.availableWithoutBorrow >= 5)
                    //        {
                    //            Console.WriteLine("Buy Price: " + buyingPrice);
                    //            Console.WriteLine("Buy Success!");
                    //            Console.WriteLine("###########################################");
                    //            isBought = true;
                    //            i = 1;
                    //        }
                    //    }
                    //}
                    #endregion

                        #region test Buying
                        var nowPrice = await GetPrice(api, ins);
                        var bid = nowPrice.bid;
                        var ask = nowPrice.ask;

                        // 2022.05.26 todo check the line is going down then buy maybe will be greater chance to have profit
                        if (!isOrdering)
                        {
                            //Console.WriteLine("ask price:" + ask);
                            //Console.WriteLine("bid - ask:" + (bid - ask));

                            if (ask > 0)
                            {
                                _askPrice_buy = ask;
                                // todo...Call Order Api

                                // todo...Check really ordering

                                isOrdering = true;

                                // todo...wait for 50 times then cancel and order again until bought
                            }
                            if (i == 1 && isOrdering)
                                Console.WriteLine("Buying price:" + _askPrice_buy + ", waiting for buying...");
                            if (i == 50 && isOrdering)
                            {
                                // cancel odering and rebuy
                            }
                            i++;
                        }

                        // if bought success
                        // todo...check really bought
                        if (_askPrice_buy >= ask)
                        {
                            var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
                            Console.WriteLine("NowTime: " + nowTime);
                            Console.WriteLine("Bought Success!");
                            Console.WriteLine("Bought Price: " + _askPrice_buy);
                            isOrdering = true;
                            isBought = true;
                            i = 1;
                        }
                        
                        #endregion
                    }
                }
                #endregion

                #region Sell                
                while (isBought)
                {
                    #region Selling
                    //    // Sell Condition
                    //    var getApeBalance = api.GetBalancesAsync().Result;
                    //    BalanceResult ApeBalanceResult = JsonConvert.DeserializeObject<BalanceResult>(getApeBalance);
                    //    var ApeBalanceList = ApeBalanceResult.result;
                    //    foreach (var item in ApeBalanceList)
                    //    {
                    //        if (item.coin == "APE")
                    //        {
                    //            if (item.availableWithoutBorrow >= 10)
                    //            {
                    //                if (!isSelling)
                    //                {
                    //                    _askPrice_sell = await GetPrice(api, ins);
                    //                    if (wantSellPrice > 0 && _askPrice_sell >= wantSellPrice)
                    //                    {
                    //                        //var rSell = api.PlaceOrderAsync(ins, SideType.sell, _askPrice_sell, OrderType.limit, item.availableWithoutBorrow ?? 0, false).Result;
                    //                        isSelling = true;
                    //                        j = 1;
                    //                        //OrderResult = JsonConvert.DeserializeObject<OrderResult>(rSell);
                    //                        //if (j == 1) { OrderID = OrderResult.result.id; }
                    //                        #region need to sheck if really sold (using while)
                    //                        #endregion
                    //                        Console.WriteLine("Selling Price: " + askPrice_sell + ", waiting for selling...");
                    //                    }
                    //                }
                    //                else if (isSelling && alreadySelling)
                    //                {
                    //                    Console.WriteLine("Selling price:" + askPrice_sell + ", waiting for selling...");
                    //                    alreadySelling = false;
                    //                }

                    //                #region Lowewr the selling price (Not using...)
                    //                //if (j == 1 && alreadySelling)
                    //                //    Console.WriteLine("Selling price:" + askPrice_sell + ", waiting for selling...");
                    //                //else if (j == 10000)
                    //                //{
                    //                //    var cancel = api.CancelOrderAsync(OrderID);
                    //                //    Console.WriteLine("Order canceled!");
                    //                //    _askPrice_sell = await GetPrice(api, ins);
                    //                //    askPrice_sell = _askPrice_sell;

                    //                //    await Task.Delay(1000);
                    //                //    var rSell = api.PlaceOrderAsync(ins, SideType.sell, askPrice_sell ?? 0, OrderType.limit, item.availableWithoutBorrow ?? 0, false).Result;
                    //                //    isSelling = true;
                    //                //    OrderResult = JsonConvert.DeserializeObject<OrderResult>(rSell);
                    //                //    OrderID = OrderResult.result.id;
                    //                //    Console.WriteLine("Selling price:" + askPrice_sell + ", waiting for selling...");
                    //                //    j = 0;
                    //                //}
                    //                #endregion

                    //                j++;
                    //            }
                    //            else if (item.availableWithoutBorrow < 1)
                    //            {
                    //                Console.WriteLine("Sell Success!");
                    //                Console.WriteLine("###########################################");
                    //                isSelling = false;
                    //                isBought = false;
                    //                isOrdering = false;
                    //                j = 1;
                    //            }
                    //        }
                    //    }
                    #endregion

                    #region test Selling                
                    var nowPrice = await GetPrice(api, ins);

                    if (!isSelling)
                    {
                        if (_askPrice_buy > 0 && nowPrice.bid > _askPrice_buy + 0.01)
                        {
                            _askPrice_sell = nowPrice.bid;

                            // todo...Call Order Api

                            // todo...Check selling order is existed

                            isSelling = true;
                            Console.WriteLine("Selling Price: " + _askPrice_sell + ", waiting for selling...");
                        }
                    }
                    // todo...check really sold
                    else
                    {
                        if (_askPrice_sell <= nowPrice.bid)
                        {
                            var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
                            Console.WriteLine("NowTime: " + nowTime);
                            Console.WriteLine("Sell Success!");
                            Console.WriteLine("###########################################");
                            _askPrice_sell = 0;
                            isSelling = false;
                            isBought = false;
                            isOrdering = false;
                        }
                    }
                    #endregion

                }
                #endregion

            }
        }
        #endregion

        #region GetHistoryPrices
        private static bool GetHistoryPrices(FtxRestApi api, string ins)
        {
            #region declare
            var priceList = new List<Price>();
            var price = new Price();
            var i = -1;
            var j = 0;
            var k = 4;
            double high = 0;
            double low = 0;
            double open = 0;
            double close = 0;
            string color;
            bool canBuy = false;
            #endregion

            #region Get history bar prices
            var dateStart = DateTime.UtcNow.AddMinutes(-3);
            var dateEnd = DateTime.UtcNow.AddMinutes(0);
            var gethistoryPrice = api.GetHistoricalPricesAsync1(ins, 60, 30, dateStart, dateEnd).Result;
            HistoryPrice historyPrices = JsonConvert.DeserializeObject<HistoryPrice>(gethistoryPrice);
            var priceResult = historyPrices.result;
            foreach (var item in priceResult)
            {
                var tempData = new Price();
                var _color = string.Empty;

                if (item.open < item.close)
                    _color = "Green";
                else if (item.open > item.close)
                    _color = "Red";
                else
                    _color = "White";

                tempData.startTime = item.startTime;
                tempData.open = item.open;
                tempData.high = item.high;
                tempData.low = item.low;
                tempData.close = item.close;
                tempData.color = _color;

                //var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
                //Console.WriteLine("StartTime: " + tempData.startTime);
                //Console.WriteLine("Color: " + tempData.color);
                //Console.WriteLine("O: " + tempData.open + " H: " + tempData.high + " L: " + tempData.low + " C: " + tempData.close);
                //Console.WriteLine("###########################################");

                priceList.Add(tempData);

                open = open + item.open;
                high = high + item.high;
                low = low + item.low;
                close = close + item.close;
            }

            if (priceList.Count >= 3)
            {
                if (priceList[0].color == "Red" && priceList[1].color == "Red" && priceList[2].color == "Green")
                {
                    var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
                    Console.WriteLine("Time: " + nowTime);
                    Console.WriteLine("true");
                    return true;
                }
            }

            //Console.WriteLine(priceList.Count);
            #endregion

            #region decide up or down
            // average price
            //var aveOpen = open / priceList.Count;
            //var aveHigh = high / priceList.Count;
            //var aveLow = low / priceList.Count;
            //var aveClose = close / priceList.Count;

            //if (aveOpen < aveClose)
            //    color = "Green";
            //else if (aveOpen > aveClose)
            //    color = "Red";
            //else
            //    color = "White";

            //if (priceList.Count == 4)
            //{
            //    if (priceList[1].color == "Red" && priceList[2].color == "Red" && priceList[2].color == "Green")
            //    {
            //        Console.WriteLine("True");

            //        //var nowTime = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
            //        //Console.WriteLine("NowTime: " + nowTime);
            //        //Console.WriteLine("###############Color: " + color + "##################");
            //        //Console.WriteLine("###############CanBuy: " + canBuy + "################");
            //        //Console.WriteLine("################Average####################");
            //        //Console.WriteLine("O: " + aveOpen + " H: " + aveHigh + " L: " + aveLow + " C: " + aveClose);
            //        //Console.WriteLine("###########################################");

            //        // return average model
            //        //price.open = aveOpen;
            //        //price.high = aveHigh;
            //        //price.low = aveLow;
            //        //price.close = aveClose;

            //        return true;
            //    }
            //}
            #endregion

            return false;
        }
        #endregion

        #region GetBalance (Not using)
        private static async Task GetBalance(FtxRestApi api, double? firstBalance, int buyTimes, double? sellProfit, double? totalProfit)
        {
            var getBalance1 = api.GetBalancesAsync().Result;
            BalanceResult BalanceResult1 = JsonConvert.DeserializeObject<BalanceResult>(getBalance1);
            var BalanceList1 = BalanceResult1.result;
            foreach (var item in BalanceList1)
            {
                if (item.coin == "USD")
                {
                    Console.WriteLine("Round: " + buyTimes);
                    Console.WriteLine("Coin: " + item.coin + ", UsdValue: " + item.usdValue);
                    firstBalance = firstBalance == 0 ? item.usdValue : firstBalance;
                    Console.WriteLine("Profit: " + (item.usdValue - firstBalance));
                    sellProfit = item.usdValue - firstBalance;
                    totalProfit = totalProfit + sellProfit;
                    Console.WriteLine("Total Profit: " + totalProfit);
                    Console.WriteLine("###########################################");
                    firstBalance = item.usdValue;
                }
            }
        }
        #endregion

        #region GetPrice
        private static async Task<getPrice> GetPrice(FtxRestApi api, string ins)
        {
            await Task.Delay(1000);
            var result = new getPrice();
            double askprice = 0;
            var buyMKPrice = api.GetSingleMarketsAsync(ins).Result;
            MarketResult MarketResult_Buy = JsonConvert.DeserializeObject<MarketResult>(buyMKPrice);
            var Market_Buy = MarketResult_Buy.result;
            result.ask = Market_Buy.ask ?? 0;
            result.bid = Market_Buy.bid ?? 0;
            return result; 
        }
        #endregion

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
            Console.WriteLine(r21);
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
            Console.WriteLine(r28);
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
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetAuthRequest(client));
                wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orderbook", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("trades", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("ticker", ins));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("fills"));
                //wsApi.SendCommand(FtxWebSockerRequestGenerator.GetSubscribeRequest("orders"));


            };

            await wsApi.Connect();
        }
        #endregion
    }
}
