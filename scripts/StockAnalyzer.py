server = '(localdb)\MSSQLLocalDB'
databaseName ='C:\\Users\\gusta\\source\\repos\\StockToDatabase\\StockToDatabase\\StockRecordDb.mdf'
tableName = 'dbo.StockTable'

import pyodbc
#import re
import datetime
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.dates as mdates

conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};'
                      'Server=(localdb)\MSSQLLocalDB;'
                      'Database=stockDb;'
                      'Trusted_Connection=yes;')

cursor = conn.cursor()
nRows = 2
nCols = 4
#cursor.execute('SELECT * FROM stockDb.dbo.StockTable')
# RecordId, RecordDate, StockName, Price, PricePerEarning, PricePerCapital, Yield
# ^0 7      ^1  8    9  ^2         ^3   10 ^4         11   ^5     12        ^6
# ProfitMargin, RSI, TechnicalAnalysis, DividendDate, ReportDate, PriceMissing)
datePosition= 1
pricePosition = 3
pePosition = 4
pcPosition = 5
yieldPosition = 6
pmPosition = 7
rsiPosition = 8
dividendDatePosition = 10
cursor.execute("SELECT * FROM stockDb.dbo.StockTable WHERE StockName = 'Peab B'")

resultList = list(cursor.fetchall())
print(resultList)
dateList = [x[datePosition] for x in resultList]
priceList = [x[pricePosition] for x in resultList]
priceList = [float(n) for n in priceList]
peList = [x[pePosition] for x in resultList]
pcList = [x[pcPosition] for x in resultList]
yieldList = [x[yieldPosition] for x in resultList]
pmList = [x[pmPosition] for x in resultList]
rsiList = [x[rsiPosition] for x in resultList]
divDateList = [x[dividendDatePosition] for x in resultList]
#print([priceList, peList])
#print(np.divide(priceList, peList))
timeToDividend = np.subtract(divDateList, dateList)
print(timeToDividend)
print(timeToDividend[1].days)
daysToDividend = [x.days for x in timeToDividend]
print(daysToDividend)
fig, axs = plt.subplots(nRows, nCols, constrained_layout=True, figsize=(16,8))

locator = mdates.AutoDateLocator(minticks=3, maxticks=7)
formatter = mdates.ConciseDateFormatter(locator)

plt.suptitle("Peab B")
axs[0,0].plot(dateList, np.divide(priceList, peList), 'r+')
axs[0,0].set_ylim(0,max(np.divide(priceList, peList)*1.1))
axs[0,0].set_title("Estimated Profit per Share")
axs[0,0].set_ylabel('Estimated Profit')
    
axs[1,0].plot(dateList, priceList, 'r+')
axs[1,0].set_ylim(0,max(priceList)*1.1)
axs[1,0].set_title("Stock Price, SEK")
axs[1,0].set_ylabel('Price')

axs[0,1].plot(dateList, peList, 'r+')
axs[0,1].set_ylim(0,max(peList)*1.1)
axs[0,1].set_title("Price/Earnings")
axs[0,1].set_ylabel('P/E')

axs[1,1].plot(dateList, pcList, 'r+')
axs[1,1].set_ylim(0,max(pcList)*1.1)
axs[1,1].set_title("Price per Capital")
axs[1,1].set_ylabel('p/c')

axs[0,2].plot(dateList, yieldList, 'r+')
axs[0,2].set_ylim(0,max(yieldList)*1.1)
axs[0,2].set_title("Yield, %")
axs[0,2].set_ylabel('Yield')

axs[1,2].plot(dateList, pmList, 'r+')
axs[1,2].set_ylim(0,max(pmList)*1.1)
axs[1,2].set_title("Profit Margin, %")
axs[1,2].set_ylabel('pm')

axs[0,3].plot(dateList, rsiList, 'r+')
axs[0,3].set_ylim(0,max(rsiList)*1.1)
axs[0,3].set_title("Relative Strength Indicator")
axs[0,3].set_ylabel('RSI')

axs[1,3].plot(dateList, daysToDividend, 'r+')
axs[1,3].set_title("Days to (+)/since(-) Dividend")
axs[1,3].set_ylabel('Days')


for iii in range(0,nRows):
  for jjj in range(0, nCols):
    axs[iii,jjj].set_xlabel('Date')
    axs[iii,jjj].xaxis.set_major_locator(locator) 
    axs[iii,jjj].xaxis.set_major_formatter(formatter)
plt.show()
