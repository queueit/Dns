Dns
=======
An asynchronous DNS resolver component written by C#.

Part of the code from [DNS.NET Resolver](http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C),this project wrap it to support .net async feature.

```
nuget install Yamool.Net.DNS
```

Features
===
- TCP or UDP protocol to query
- Supports all of [DNS record types](https://en.wikipedia.org/wiki/List_of_DNS_record_types)

Usage
====
```c#
var DefaultDnsServer = new EndPoint[] { new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53) };
var host = "www.google.com";
var resolver = new Resolver(DefaultDnsServer)
{
	Timeout = 1000,
	Retries = 3,
	TransportType = TransportType.Tcp
};
var response = await resolver.QueryAsync(host, QTYPE.A);
```
```
;; Got answer:
;; ->>Header<<- opcode: Query, status: NoError, id: 30693
;; flags:  qr rd ra; QUERY: 1, ANSWER: 1, AUTHORITY: 0, ADDITIONAL: 0

;; QUESTION SECTION:
;www.google.com.                 	IN	A

;; ANSWER SECTION:
www.google.com.                  28	IN	A	216.58.221.100

;; Query time: 101 msec
;; SERVER: 8.8.8.8#53(8.8.8.8)
;; WHEN: Mon Jul 13 23:12:30 2015
```
MX
```c#
var host = "163.com";
var resolver = new Resolver(DefaultDnsServer)
{
	Timeout = 1000,                
	TransportType = TransportType.Udp
};
var response = await resolver.QueryAsync(host, QTYPE.MX).ConfigureAwait(false);
```			
```
;; Got answer:
;; ->>Header<<- opcode: Query, status: NoError, id: 29780
;; flags:  qr rd ra; QUERY: 1, ANSWER: 4, AUTHORITY: 0, ADDITIONAL: 0

;; QUESTION SECTION:
;163.com.                        	IN	MX

;; ANSWER SECTION:
163.com.                         13377	IN	MX	10 163mx02.mxmail.netease.com.
163.com.                         13377	IN	MX	50 163mx00.mxmail.netease.com.
163.com.                         13377	IN	MX	10 163mx03.mxmail.netease.com.
163.com.                         13377	IN	MX	10 163mx01.mxmail.netease.com.

;; Query time: 88 msec
;; SERVER: 8.8.8.8#53(8.8.8.8)
;; WHEN: Mon Jul 13 23:29:53 2015
```