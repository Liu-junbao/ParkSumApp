using ParkSumApp.Commands;
using ParkSumApp.Helpers;
using ParkSumApp.Models;
using ParkSumApp.Services;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ParkSumApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "出入库统计";
        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _loadedStartDate;
        private DateTime _loadedEndDate;
        private List<StatisticViewModel> _items;
        public MainWindowViewModel(IDataService service)
        {
            AccountLogStore = service.GetDataStore<AccountLog>();
            var time = DateTime.Now;
            StartTime = time.Date;
            EndTime = time.Date.AddDays(1) - TimeSpan.FromSeconds(1);
            SearchCommand = new Command(OnSearch);
            StopCommand = new Command(OnStop);
            ExportCommand = new Command(OnExport);
        }
        public IDataStore<AccountLog> AccountLogStore { get; }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }
        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }
        public bool IsStoped { get; private set; }
        public Command SearchCommand { get; }
        public Command StopCommand { get; }
        public Command ExportCommand { get; }
        /// <summary>
        /// 统计列表
        /// </summary>
        public List<StatisticViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        private async Task OnSearch()
        {
            var start = StartTime;
            var end = EndTime;

            //日期部分
            var startDate = start.Date;
            var endDate = end.Date;

            //时段部分
            var startTimeOfDay = start.TimeOfDay;
            var endTimeOfDay = end.TimeOfDay;

            var items = new List<StatisticViewModel>();

            var date = startDate;
            while (date <= endDate)
            {
                items.Add(new StatisticViewModel(date + startTimeOfDay, date + endTimeOfDay));
                date = date.AddDays(1);
            }

            Items = items;

            _loadedStartDate = startDate;
            _loadedEndDate = endDate;

            var store = AccountLogStore;

            IsStoped = false;
            foreach (var item in items)
            {
                if (IsStoped) break;
                await item.Calculate(store);
            }
        }
        private async Task OnStop()
        {
            IsStoped = true;
        }

        private async Task OnExport()
        {
            var items = Items;
            if (items == null || items.Count == 0)
            {
                MessageBox.Show("当前无数据!");
                return;
            }

            var startDate = _loadedStartDate;
            var endDate = _loadedEndDate;

            var ret = await ExcelHelper.ExportWithDialog($"{startDate:yyyy-MM-dd} -- {endDate:yyyy-MM-dd} 出入库统计", items, "统计数据", new Dictionary<string, Func<StatisticViewModel, object>>()
            {
                {"日期",i=>$"{i.StartTime:yyyy-MM-dd dddd}"},
                {"起始时间",i=>$"{i.StartTime:HH:mm:ss}"},
                {"截止时间",i=>$"{i.EndTime:HH:mm:ss}"},
                {"总数",i=>$"{i.Count}"},
                {"存取",i=>$"{i.ParkInAndOutCount}"},
                {"存车",i=>$"{i.ParkInCount}"},
                {"取车",i=>$"{i.ParkOutCount}"},
                {"存车平均",i=>$"{i.ParkInAverageTime}"},
                {"取车平均",i=>$"{i.ParkOutAverageTime}"},
                {"存车最小",i=>$"{i.ParkInMiniTime}"},
                {"存车最大",i=>$"{i.ParkInMaxiTime}"},
                {"取车最小",i=>$"{i.ParkOutMiniTime}"},
                {"取车最大",i=>$"{i.ParkOutMaxiTime}"},
            });
            if (ret == true)
            {
                MessageBox.Show("导表成功!");
            }
            else if (ret == false)
            {
                MessageBox.Show("导表失败!");
            }
        }
    }


    public class StatisticViewModel : BindableBase
    {
        private double? _count;
        private double? _parkInAverageTime;
        private double? _parkInMiniTime;
        private double? _parkInMaxiTime;
        private double? _parkOutAverageTime;
        private double? _parkOutMiniTime;
        private double? _parkOutMaxiTime;
        private double? _parkInCount;
        private double? _parkOutCount;
        private double? _parkInAndOutCount;
        private int _status;
        private List<AccountLogViewModel> _items;
        public StatisticViewModel(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        /// <summary>
        /// 总数
        /// </summary>
        public double? Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }
        /// <summary>
        /// 存车数
        /// </summary>
        public double? ParkInCount
        {
            get { return _parkInCount; }
            set { SetProperty(ref _parkInCount, value); }
        }
        /// <summary>
        /// 取车数
        /// </summary>
        public double? ParkOutCount
        {
            get { return _parkOutCount; }
            set { SetProperty(ref _parkOutCount, value); }
        }
        /// <summary>
        /// 存取同段
        /// </summary>
        public double? ParkInAndOutCount
        {
            get { return _parkInAndOutCount; }
            set { SetProperty(ref _parkInAndOutCount, value); }
        }
        /// <summary>
        /// 存车平均值
        /// </summary>
        public double? ParkInAverageTime
        {
            get { return _parkInAverageTime; }
            set { SetProperty(ref _parkInAverageTime, value); }
        }
        /// <summary>
        /// 存车最小值
        /// </summary>
        public double? ParkInMiniTime
        {
            get { return _parkInMiniTime; }
            set { SetProperty(ref _parkInMiniTime, value); }
        }
        /// <summary>
        /// 存在最大值
        /// </summary>
        public double? ParkInMaxiTime
        {
            get { return _parkInMaxiTime; }
            set { SetProperty(ref _parkInMaxiTime, value); }
        }
        /// <summary>
        /// 取车平均值
        /// </summary>
        public double? ParkOutAverageTime
        {
            get { return _parkOutAverageTime; }
            set { SetProperty(ref _parkOutAverageTime, value); }
        }
        /// <summary>
        /// 取车最小值
        /// </summary>
        public double? ParkOutMiniTime
        {
            get { return _parkOutMiniTime; }
            set { SetProperty(ref _parkOutMiniTime, value); }
        }
        /// <summary>
        /// 取车最大值
        /// </summary>
        public double? ParkOutMaxiTime
        {
            get { return _parkOutMaxiTime; }
            set { SetProperty(ref _parkOutMaxiTime, value); }
        }
        /// <summary>
        /// 加载状态
        /// </summary>
        public int Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        /// <summary>
        /// 明细
        /// </summary>
        public List<AccountLogViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        public async Task Calculate(IDataStore<AccountLog> dataStore)
        {
            Status = 1;
            var from = StartTime;
            var to = EndTime;
            Items = (await dataStore.GetItems(i => (i.CardInTime != null && i.CardInTime >= from && i.CardInTime < to) || (i.CardOutTime != null && i.CardOutTime >= from && i.CardOutTime < to))).Select(i => new AccountLogViewModel(i)).ToList();
            var items = Items;
            if (items == null || items.Count == 0)
            {
                Count = null;
                ParkInCount = null;
                ParkOutCount = null;
                ParkInAverageTime = null;
                ParkInMiniTime = null;
                ParkInMaxiTime = null;
                ParkOutAverageTime = null;
                ParkOutMiniTime = null;
                ParkOutMaxiTime = null;
                Status = 2;
                return;
            }

            int parkinCount = 0, parkoutCount = 0, parkInAndOuntCount = 0;
            var parkInTotal = new List<double>();
            var parkOutTotal = new List<double>();
            double? parkInMin = null, parkInMax = null;
            double? parkOutMin = null, parkOutMax = null;
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Model.PlateNumber))
                {
                    item.IsParkInOver = true;
                }

                var parkInTime = item.ParkInTime;

                if (parkInTime < 60 || parkInTime > 25 * 60)//不在范围内 1分钟 ~ 25分钟
                {
                    item.IsParkInOver = true;
                }

                if (item.IsParkInOver == false)
                {
                    parkInTotal.Add(parkInTime);

                    if (parkInMin == null)
                        parkInMin = parkInTime;
                    else if (parkInTime < parkInMin)
                        parkInMin = parkInTime;

                    if (parkInMax == null)
                        parkInMax = parkInTime;
                    else if (parkInMax < parkInTime)
                        parkInMax = parkInTime;

                }

                var parkOutTime = item.ParkOutTime;

                if (parkOutTime < 60 || parkOutTime > 60 * 60)//不在范围内 1分钟 ~ 60分钟
                {
                    item.IsParkOutOver = true;
                }

                if (item.IsParkOutOver == false)
                {
                    parkOutTotal.Add(parkOutTime);

                    if (parkOutMin == null)
                        parkOutMin = parkOutTime;
                    else if (parkOutTime < parkOutMin)
                        parkOutMin = parkOutTime;

                    if (parkOutMax == null)
                        parkOutMax = parkOutTime;
                    else if (parkOutMax < parkOutTime)
                        parkOutMax = parkOutTime;
                }

                var isParkIn = item.Model.CardInTime != null && item.Model.CardInTime >= from && item.Model.CardInTime < to;
                var isParkOut = item.Model.CardOutTime != null && item.Model.CardOutTime >= from && item.Model.CardOutTime < to;
                if (isParkIn && item.IsParkInOver == false)
                {
                    parkinCount++;
                }
                if (isParkOut && item.IsParkOutOver == false)
                {
                    parkoutCount++;
                }

                if (isParkIn && isParkOut && item.IsParkInOver == false && item.IsParkOutOver == false)
                {
                    parkInAndOuntCount++;
                }
            }

            var count = parkinCount + parkoutCount;

            ParkInCount = parkinCount == 0 ? null : (double?)parkinCount;
            ParkOutCount = parkoutCount == 0 ? null : (double?)parkoutCount;
            ParkInAndOutCount = parkInAndOuntCount == 0 ? null : (double?)parkInAndOuntCount;
            Count = count == 0 ? null : (double?)count;

            if (parkInTotal.Count > 0)
                ParkInAverageTime = Math.Round(parkInTotal.Sum() / parkInTotal.Count, 2);
            if (parkOutTotal.Count > 0)
                ParkOutAverageTime = Math.Round(parkOutTotal.Sum() / parkOutTotal.Count, 2);

            ParkInMiniTime = parkInMin != null ? (double?)Math.Round(parkInMin.Value, 2) : null;
            ParkInMaxiTime = parkInMax != null ? (double?)Math.Round(parkInMax.Value, 2) : null;
            ParkOutMiniTime = parkOutMin != null ? (double?)Math.Round(parkOutMin.Value, 2) : null;
            ParkOutMaxiTime = parkOutMax != null ? (double?)Math.Round(parkOutMax.Value, 2) : null;
            Status = 2;
        }
    }

    public class AccountLogViewModel : BindableBase
    {

        private bool _isParkInOver;
        private bool _isParkOutOver;
        public AccountLogViewModel(AccountLog log)
        {
            Model = log;
            if (log.CardInTime != null && log.ParkInTime != null)
            {
                ParkInTime = (log.ParkInTime.Value - log.CardInTime.Value).TotalSeconds;
            }
            if (log.CardOutTime != null && log.ParkOutTime != null)
            {
                ParkOutTime = (log.ParkOutTime.Value - log.CardOutTime.Value).TotalSeconds;
            }
        }
        public AccountLog Model { get; }
        public double ParkInTime { get; set; }
        public double ParkOutTime { get; set; }
        /// <summary>
        /// 存车超时/车牌为空
        /// </summary>
        public bool IsParkInOver
        {
            get { return _isParkInOver; }
            set { SetProperty(ref _isParkInOver, value); }
        }
        /// <summary>
        /// 取车超时
        /// </summary>
        public bool IsParkOutOver
        {
            get { return _isParkOutOver; }
            set { SetProperty(ref _isParkOutOver, value); }
        }
    }
}
