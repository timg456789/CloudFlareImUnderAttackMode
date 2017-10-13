using System;
using CloudFlareImUnderAttackMode;
using NUnit.Framework;

namespace CloudFlareImUnderAttackModeTests
{
    class NationalGalleryOfArtTest
    {
        /// <summary>
        /// 2017-12-10
        /// The website isn't in I'm under attack mode. There is no challenge question presented with a 503 on the initial home page request.
        /// </summary>
        [Test]
        public void Test_The_National_Gallery_Of_Art()
        {
            var uri = new Uri("http://images.nga.gov/en/page/show_home_page.html");
            var client= new CloudFlareImUnderAttackModeHttpClientFactory().Create(uri);
        }
    }
}
