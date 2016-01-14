# GetTicket_IOT
---
## 软件介绍
这个程序主要是用在Windows IOT设备上的,定时刷新12306网站火车票,当刷到票时点亮一个LED,提醒有票.

## 使用方式

修改MainPage.xaml.cs 中的queryUrl,需要修改三个参数,1是日期,2是起始站的代码,3是终点站的代码

通过修改timer.Interval可以修改时间间隔

## ToDO
>* 写一个配置Server,可以方便的控制IOT上刷票的参数等
>* ...

## License

MIT