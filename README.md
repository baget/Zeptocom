# Zeptocom
> Yet another quick and dirty Serial (COM Port) Console Terminal, Written in .NET 5

[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/baget/zeptocom/blob/master/LICENSE)

This project is development to be quick in order to provide a console Serial terminal that run in Windows Console (command line)
it also support [Windows Terminal](https://github.com/microsoft/terminal)

![](img/screenshot1.png)

## Usage example

the application get the COM Port number as parameter and will open it with 115200bps unless changed with ```--baudrate``` switch

``` 
Examples:
1) zeptocom.app.exe COM1 --baudrate 9600
2) zeptocom.app.exe COM4
```

## Development setup

Open in Visual Studio 2019 and build, or compile it from command line

```
cd Zeptocom\Zeptocom.App
dotnet build
dotnet run
```

## Release History

* 0.0.1
    * Work in progress

## Meta

Oren Weil – [@bagetx](https://twitter.com/bagetx)

Distributed under the MTI license. See ``LICENSE`` for more information.

[https://github.com/baget/baget](https://github.com/baget/)

## Contributing

1. Fork it (<https://github.com/baget/zeptocom/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

<!-- Markdown link & img dfn's -->
[npm-image]: https://img.shields.io/npm/v/datadog-metrics.svg?style=flat-square
[npm-url]: https://npmjs.org/package/datadog-metrics
[npm-downloads]: https://img.shields.io/npm/dm/datadog-metrics.svg?style=flat-square
[travis-image]: https://img.shields.io/travis/dbader/node-datadog-metrics/master.svg?style=flat-square
[travis-url]: https://travis-ci.org/dbader/node-datadog-metrics
[wiki]: https://github.com/yourname/yourproject/wiki
