using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Winmine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Button b;               //按钮
        int rows = 10;          //行
        int cols = 10;          //列
        int hard = 10;           //难度    10\50\500\1000
        int number;             //被点按钮周围的雷数
        List<int> index = new List<int>();      //雷的索引
        Random ran = new Random();              //产生雷的随机索引
        int ind;                                //接引产生的随机索引
        int max;                              //最大索引值
        int x,y;                               //被点按钮的索引
        int tag;                             //标签的值
        MenuItem mi;                        //菜单
        DispatcherTimer dt = new DispatcherTimer();
        bool isCheck=false;                               //是否检查展开完全
        int hasBlank;                                   //是否还有空白的按钮
        double doubleTime;                                    //时间
        int intScore;                                   //分数
        int intClear;                                   //扫雷数
        int right;                                      //右键执行的一个操作
        List<Button> rb = new List<Button>();           //右键标记过的按钮
        List<int> ri = new List<int>();                 //右键按钮需要执行的操作
        int isEnd;                                      //检测游戏是否胜利结束
        bool startBool = false;                             //是否开始游戏
        DispatcherTimer loadGame=new DispatcherTimer();
        List<Button> mb = new List<Button>();           //提示过的按钮
        //List<int> gridIndexList = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Start game
        public void gameStart()
        {
            //gridIndexList.Clear();
            //初始化网格
            for (int i = 0; i < rows; i++)
            {
                gameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < cols; j++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            //添加雷的索引
            index.Clear();
            max = cols * rows;
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        gridIndexList.Add(i * rows + j);
            //    }
            //}
            while (true)
            {
                ind = ran.Next(0, max);
                if (!index.Contains(ind))
                {
                    index.Add(ind);
                }
                //ind = ran.Next(0, gridIndexList.Count);
                //index.Add(gridIndexList[ind]);
                //gridIndexList.RemoveAt(ind);
                if (index.Count >= hard)
                {
                    break;
                }
            }
            //初始化游戏
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    b = new Button();
                    b.Style = this.Resources["standard"] as Style;
                    b.Tag = "0";
                    if (index.Contains(i * cols + j))
                    {
                        b.Tag = "*";
                    }
                    b.Focusable = false;
                    Grid.SetColumn(b, j);
                    Grid.SetRow(b, i);
                    gameGrid.Children.Add(b);
                }
            }
            //计算数字
            for (int z = 0; z < max; z++)
            {
                b = gameGrid.Children[z] as Button;
                if (b.Tag.ToString() == "*")
                {
                    continue;
                }
                x = Grid.GetRow(b);
                y = Grid.GetColumn(b);
                number = 0;
                for(int i = -1; i < 2; i++)
                {
                    for(int j = -1; j < 2; j++)
                    {
                        if(x+i>-1 && y+j>-1 && x+i<rows && y + j < cols)
                        {
                            if((gameGrid.Children[z+i*cols+j] as Button).Tag.ToString() == "*")
                            {
                                number++;
                            }
                        }
                    }
                }
                //for (int k = 0; k < max - 1; k++)
                //{
                //    for (int i = -1; i < 2; i++)
                //    {
                //        for (int j = -1; j < 2; j++)
                //        {
                //            if (Grid.GetRow(gameGrid.Children[k]) == (x + i) && Grid.GetColumn(gameGrid.Children[k]) == (y + j))
                //            {
                //                if ((gameGrid.Children[k] as Button).Tag.ToString() == "*")
                //                {
                //                    number++;
                //                }
                //            }
                //        }
                //    }
                //}
                b.Tag = number == 0 ? " " : number.ToString();
                foreground();
            }
            //初始化数据
            doubleTime = 0;
            right = 0;
            intScore = 0;
            rb.Clear();
            ri.Clear();
        }
        #endregion

        #region Click Menu
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            mi = sender as MenuItem;
            if (mi.Name == "about")
            {
                MessageBox.Show("作者：潘滔（Pantao）\r\nQQ：735817834\r\n\r\nPS：\r\n  （1）程序有任何BUG或者界面显示问题请及时提出，方便作者改进。\r\n  （2）有好的建议也强烈建议加QQ提出。\r\n\r\n注意：版权归作者所有，侵权必究。", "作者资料",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            dt.IsEnabled = false;
            //清理网格
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            if (mi.Name == "easy")
            {
                //10p100
                hard = 10;
                this.Width = 250;
                this.Height = 320;
                rows = 10;
                cols = 10;
                //clear.Visibility = Visibility.Collapsed;
                //clearTB.Visibility = Visibility.Collapsed;
            }
            else if (mi.Name == "soso")
            {
                //50p400
                hard = 50;
                this.Width = 500;
                this.Height=560;
                rows = 20;
                cols = 20;
                this.Top = 100;
            }
            else if (mi.Name == "harder")
            {
                //200p1200
                hard = 200;
                this.Width = 900;
                this.Height = 650;
                rows = 30;
                cols = 40;
                this.Top = 50;
                this.Left = 200;
            }
            else if (mi.Name == "super")
            {
                //300p1500
                hard = 300;
                this.Width = 1100;
                this.Height = 650;
                rows = 30;
                cols = 50;
                this.Left = 100;
                this.Top = 50;
            }
            if (mi.Name != "easy")
            {
                clear.Visibility = Visibility.Visible;
                clearTB.Visibility = Visibility.Visible;
            }
            total.Text = hard.ToString();
            dt.IsEnabled = true;
            startBool = true;
            dt.Start();
            if (hard == 10)
            {
                clear.Visibility = Visibility.Collapsed;
                clearTB.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Click Button
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            b = sender as Button;
            if (mb.Count > 0)
            {
                for (int i = 0; i < mb.Count; i++)
                {
                    (mb[i] as Button).Content = null;
                }
                mb.Clear();
            }
            if (b.Tag.ToString() == "*")
            {
                //游戏结束
                for(int i = 0; i < max; i++)
                {
                    b = gameGrid.Children[i] as Button;
                    if (b.Tag.ToString() == "*")
                    {
                        b.Template = this.Resources["boom"] as ControlTemplate;
                    }
                    else
                    {
                        b.Content = b.Tag.ToString();
                    }
                    b.IsEnabled = false;
                }
                dt.IsEnabled = false;
                MessageBox.Show("不好意思，你玩完啦！", "哭，你输了", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            b.Content = b.Tag.ToString();
            b.Template = this.Resources["blank"] as ControlTemplate;
            //展开
            isCheck = true;
            b.IsEnabled = false;
            intScore+=(hard/10);
            e.Handled = true;
        }
        #endregion

        #region Windows Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.IsEnabled = false;
            dt.Tick += Dt_Tick;
            loadGame.Interval = TimeSpan.FromSeconds(1);
            loadGame.IsEnabled = false;
            loadGame.Tick += LoadGame_Tick;
        }
        #endregion

        #region Load Game Thread
        private void LoadGame_Tick(object sender, EventArgs e)
        {
            //计算数字
            for (int z = 0; z < max; z++)
            {
                b = gameGrid.Children[z] as Button;
                if (b.Tag.ToString() == "*")
                {
                    continue;
                }
                x = Grid.GetRow(b);
                y = Grid.GetColumn(b);
                number = 0;
                for (int k = 0; k < max - 1; k++)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (Grid.GetRow(gameGrid.Children[k]) == (x + i) && Grid.GetColumn(gameGrid.Children[k]) == (y + j))
                            {
                                if ((gameGrid.Children[k] as Button).Tag.ToString() == "*")
                                {
                                    number++;
                                }
                            }
                        }
                    }
                }
                b.Tag = number == 0 ? " " : number.ToString();
                foreground();
            }
            this.Title = "扫雷";
            loadGame.IsEnabled = false;
        }
        #endregion

        #region Game Thread
        private void Dt_Tick(object sender, EventArgs e)
        {
            if (startBool)
            {
                intClear = 0;
                this.Title = "扫雷   ------------------数据加载中----------------------------------------------------------";
                gameStart();
                //loadGame.IsEnabled = true;
                //this.Title = "扫雷   -------------数据加载中（计算每个方块的数值）------------------------------------------------------";
                //loadGame.Start();
                this.Title = "扫雷";
                startBool = false;
            }
            if (isCheck)
            {
                hasBlank = 0;
                isEnd = 0;
                for(int z = 0; z < max; z++)
                {
                    b = gameGrid.Children[z] as Button;
                    if (b.IsEnabled == false)
                    {
                        isEnd++;
                        if(b.Tag.ToString()==" ")
                        {
                            calcuteSpread(b);
                        }
                    }
                }
                if (isEnd >= (max - hard))
                {
                    MessageBox.Show("不错哦，赢了，再来一局吧", "厉害了", MessageBoxButton.OK, MessageBoxImage.Information);
                    for(int z = 0; z < max; z++)
                    {
                        (gameGrid.Children[z] as Button).IsEnabled = false;
                    }
                    dt.IsEnabled = false;
                }
                if (hasBlank > -1)
                {
                    isCheck = false;
                }
            }
            doubleTime+=dt.Interval.TotalMilliseconds;
            time.Text = (doubleTime/100).ToString("#0.00");
            score.Text = intScore + "分  ";
            clear.Text = intClear + "个  ";
        }
        #endregion

        #region Calcute Spread
        public void calcuteSpread(Button cb)
        {
            x = Grid.GetRow(cb);
            y = Grid.GetColumn(cb);
            for (int k = 0; k < max; k++)
            {
                b = gameGrid.Children[k] as Button;
                if (!b.IsEnabled)
                {
                    continue;
                }
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Grid.GetRow(b) == (x + i) && Grid.GetColumn(b) == (y + j))
                        {
                            hasBlank--;
                            b.IsEnabled = false;
                            b.Template = this.Resources["blank"] as ControlTemplate;
                            if (b.Tag.ToString() != " " && b.Tag.ToString()!="*")
                            {
                                intScore+=(hard/10);
                                foreground();
                                b.Content = b.Tag.ToString();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Foreground Color
        public void foreground()
        {
            //添加前景颜色
            try
            {
                tag = Int32.Parse(b.Tag.ToString());
            }
            catch
            {
                tag = 0;
            }
            if (tag == 1)
            {
                b.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (tag == 2)
            {
                b.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else if (tag == 3)
            {
                b.Foreground = new SolidColorBrush(Colors.YellowGreen);
            }
            else if (tag == 4)
            {
                b.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (tag == 5)
            {
                b.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else if (tag == 6)
            {
                b.Foreground = new SolidColorBrush(Colors.Violet);
            }
            else if (tag == 7)
            {
                b.Foreground = new SolidColorBrush(Colors.Indigo);
            }
            else if (tag == 8)
            {
                b.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        #endregion

        #region Mouse Right Button Down
        public void Button_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            b = sender as Button;
            if (rb.Contains(b))
            {
                right = ri[rb.IndexOf(b)];
            }
            else
            {
                rb.Add(b);
                right = 0;
                ri.Add(right);
            }
            ri[rb.IndexOf(b)] = right + 1;
            if (right % 3 == 0)
            {
                b.Template = this.Resources["flag"] as ControlTemplate;
                intClear++;
            }
            else if(right%3==1)
            {
                b.Template = this.Resources["ask"] as ControlTemplate;
                intClear--;
            }
            else
            {
                b.Content = "";
                b.Template = this.Resources["resume"] as ControlTemplate;
                ri.RemoveAt(rb.IndexOf(b));
                rb.Remove(b);
            }
            e.Handled = true;
        }
        #endregion

        #region Mouse Wheel to Mention
        public void Button_MouseWheel(object sender, RoutedEventArgs e)
        {
            b = sender as Button;
            if (!mb.Contains(b))
            {
                mb.Add(b);
                b.Content = b.Tag.ToString();
                e.Handled = true;
            }
            
        }
        #endregion
    }
}
