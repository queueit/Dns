namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Yamool.Net.DNS;

    [TestFixture]
    public class ResolverTest
    {
        internal static EndPoint[] DefaultDnsServer = new EndPoint[] { new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53) };

        private Stopwatch sw;

        [TestFixtureSetUp]
        public void Init()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        [TestFixtureTearDown]
        public void Completed()
        {
            sw.Stop();
        }

        [Test]
        public async void TestQueryByTCP()
        {         
            var host = "www.google.com";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,
                Retries = 3,
                TransportType = TransportType.Tcp
            };
            var response = await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false);
            Assert.IsNotNull(response);
            Assert.AreEqual("www.google.com.", response.Answers[0].NAME);
            this.PrintResponse(response);
        }

        [Test]
        public async void TestQueryByUDP()
        {
            var host = "www.google.com";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,
                TransportType = TransportType.Udp
            };          
            var response = await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false);
            Assert.IsNotNull(response);
            Assert.AreEqual("www.google.com.", response.Answers[0].NAME);
            this.PrintResponse(response);
        }

        [Test]
        public async void TestMXRecord()
        {
            var host = "163.com";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,                
                TransportType = TransportType.Udp
            };
            var response = await resolver.QueryAsync(host, QTYPE.MX).ConfigureAwait(false);
            Assert.IsNotNull(response);            
            this.PrintResponse(response);
        }

        [Test]
        public async void TestQuery163()
        {
            var host = "www.163.com";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,
                TransportType = TransportType.Udp
            };
            var response = await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false);
            Assert.IsNotNull(response);
            Assert.AreEqual("www.163.com.", response.Answers[0].NAME);
            this.PrintResponse(response);
        }

        [Test]
        public async void TestQueryIP()
        {
            var host = "173.194.127.115";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,
                TransportType = TransportType.Udp
            };
            var response = await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false);
            Assert.IsNotNull(response);
            this.PrintResponse(response);
        }

        [Test]
        public async void TestHostEntry()
        {
            var hostName = "www.bing.com";
            var resolver = new Resolver(DefaultDnsServer);
            var hostEntry = await resolver.GetHostEntryAsync(hostName).ConfigureAwait(false);
            Assert.AreEqual("www.bing.com.", hostEntry.Aliases[0]);

            hostEntry = await resolver.GetHostEntryAsync(hostEntry.AddressList[0]).ConfigureAwait(false);           
        }

        [Test]
        public async void TestQueryInRetry()
        {
            var host = "www.google.com";
            var dnsServerList = new EndPoint[] { new IPEndPoint(IPAddress.Parse("127.0.0.2"), 53), new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53) };

            var resolver = new Resolver(dnsServerList)
            {
                Timeout = 10000,
                TransportType = TransportType.Udp,
                Retries = 2
            };
            var response = await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false);
            Assert.AreEqual("www.google.com.", response.Answers[0].NAME);
            this.PrintResponse(response);
        }

        [Test]
        public async void TestTimeoutException()
        {
            var host = "www.example.com";
            var resolver = new Resolver(DefaultDnsServer)
            {
                Timeout = 1000,
                TransportType = TransportType.Tcp,
                Retries = 1
            };
           Assert.Throws<TaskCanceledException>(async () => await resolver.QueryAsync(host, QTYPE.A).ConfigureAwait(false));            
        }
        
        private void PrintResponse(Response response)
        {           
            Console.WriteLine(";; Got answer:");

            Console.WriteLine(";; ->>Header<<- opcode: {0}, status: {1}, id: {2}",
                response.Header.OPCODE,
                response.Header.RCODE,
                response.Header.ID);
            Console.WriteLine(";; flags: {0}{1}{2}{3}; QUERY: {4}, ANSWER: {5}, AUTHORITY: {6}, ADDITIONAL: {7}",
                response.Header.QR ? " qr" : "",
                response.Header.AA ? " aa" : "",
                response.Header.RD ? " rd" : "",
                response.Header.RA ? " ra" : "",
                response.Header.QDCOUNT,
                response.Header.ANCOUNT,
                response.Header.NSCOUNT,
                response.Header.ARCOUNT);
            Console.WriteLine("");

            if (response.Header.QDCOUNT > 0)
            {
                Console.WriteLine(";; QUESTION SECTION:");
                foreach (Question question in response.Questions)
                    Console.WriteLine(";{0}", question);
                Console.WriteLine("");
            }

            if (response.Header.ANCOUNT > 0)
            {
                Console.WriteLine(";; ANSWER SECTION:");
                foreach (AnswerRR answerRR in response.Answers)
                {
                    var s=answerRR.RECORD.ToString();
                    Console.WriteLine(answerRR);
                }
                Console.WriteLine("");
            }

            if (response.Header.NSCOUNT > 0)
            {
                Console.WriteLine(";; AUTHORITY SECTION:");
                foreach (AuthorityRR authorityRR in response.Authorities)
                    Console.WriteLine(authorityRR);
                Console.WriteLine("");
            }

            if (response.Header.ARCOUNT > 0)
            {
                Console.WriteLine(";; ADDITIONAL SECTION:");
                foreach (AdditionalRR additionalRR in response.Additionals)
                    Console.WriteLine(additionalRR);
                Console.WriteLine("");
            }

            Console.WriteLine(";; Query time: {0} msec", sw.ElapsedMilliseconds);
            Console.WriteLine(";; SERVER: {0}#{1}({2})", ((IPEndPoint)response.Server).Address, ((IPEndPoint)response.Server).Port, ((IPEndPoint)response.Server).Address);
            Console.WriteLine(";; WHEN: " + response.TimeStamp.ToString("ddd MMM dd HH:mm:ss yyyy", new System.Globalization.CultureInfo("en-US")));
            //Console.WriteLine(";; MSG SIZE rcvd: " + response.MessageSize);
        }
    }
}
