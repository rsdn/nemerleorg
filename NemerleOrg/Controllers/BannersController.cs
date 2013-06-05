using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NemerleOrg.Controllers
{
  public class BannersController : Controller
  {
    [HttpGet]
    public ActionResult Index(string t, string g)
    {
      var text = string.IsNullOrEmpty(t)
        ? "DEVELOPER"
        : (t.Length > 64 ? t.Substring(0, 64) : t);
      var gamma = (g ?? "").ToLowerInvariant() == "dark" ? "Dark" : "Light";
      var sha1 = MakeHash(gamma + text);
      var filePath = Server.MapPath("~/Static/Cache/" + sha1);
      if (!System.IO.File.Exists(filePath))
        MakeBannerFile(filePath, text, gamma);
      return File(filePath, "image/png");
    }

    [HttpGet]
    public ActionResult Cache()
    {
      var imageRefs = new List<string>();
      foreach (var cacheFilePath in Directory.GetFiles(Server.MapPath("~/Static/Cache"), "*."))
        imageRefs.Add(VirtualPathUtility.ToAbsolute(Path.Combine("~/Static/Cache", Path.GetFileName(cacheFilePath))));
      return View(imageRefs);
    }

    private string MakeHash(string text)
    {
      var data = Encoding.UTF8.GetBytes(text);
      var hasher = new SHA1Managed();
      var hash = hasher.ComputeHash(data);
      var buffer = new StringBuilder(hash.Length * 2);
      foreach (var b in hash)
        buffer.Append(b.ToString("x2"));
      return buffer.ToString();
    }

    private void MakeBannerFile(string filePath, string text, string gamma)
    {
      const string glyphsText = " 1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦШЩЧЬЫЪЭЮЯабвгдеёжзийклмнопрстуфхцшщчьыъэюя`-=[]\\;',./~!@#$%^&*()_+{}|:\"<>?№";
      var bitmapSourceDirectory = Server.MapPath("~/Static/Images/" + gamma);
      using (
        Bitmap bmGlyphs = new Bitmap(Path.Combine(bitmapSourceDirectory, "Glyphs.png")),
        bmTitle = new Bitmap(Path.Combine(bitmapSourceDirectory, "Title.png")),
        bmBackgroundLeft = new Bitmap(Path.Combine(bitmapSourceDirectory, "Background_Left.png")),
        bmBackgroundMiddle = new Bitmap(Path.Combine(bitmapSourceDirectory, "Background_Middle.png")),
        bmBackgroundRight = new Bitmap(Path.Combine(bitmapSourceDirectory, "Background_Right.png")))
      {
        var glyphWidth = bmGlyphs.Width / glyphsText.Length;
        var glyphHeight = bmGlyphs.Height;
        var leftTextOffset = 74;
        var rightTextOffset = 11;
        var topTextOffset = 37;
        var textWidth = text.Length * glyphWidth;
        var backgroundAreaWidth = bmBackgroundLeft.Width - leftTextOffset + bmBackgroundRight.Width - rightTextOffset;
        int bmMiddleWidth;
        if (backgroundAreaWidth >= textWidth)
        {
          leftTextOffset += (backgroundAreaWidth - textWidth) / 2;
          bmMiddleWidth = 0;
        }
        else
          bmMiddleWidth = textWidth - backgroundAreaWidth;
        var titleLeftOffset = leftTextOffset + (textWidth - bmTitle.Width) / 2;
        var titleTopOffset = 23;
        var bannerWidth = bmBackgroundLeft.Width + bmMiddleWidth + bmBackgroundRight.Width;
        var bannerHeight = (bmBackgroundLeft.Height + bmBackgroundMiddle.Height + bmBackgroundRight.Height) / 3;

        using (var bmBanner = new Bitmap(bannerWidth, bannerHeight, PixelFormat.Format32bppPArgb))
        {
          bmBanner.SetResolution(bmGlyphs.HorizontalResolution, bmGlyphs.VerticalResolution);
          using (var g = Graphics.FromImage(bmBanner))
          {
            g.DrawImageUnscaled(bmBackgroundLeft, 0, 0);
            for (var i = bmBackgroundLeft.Width; i < bmBackgroundLeft.Width + bmMiddleWidth; i++)
              g.DrawImageUnscaled(bmBackgroundMiddle, i, 0);
            g.DrawImageUnscaled(bmBackgroundRight, bmBackgroundLeft.Width + bmMiddleWidth, 0);
            g.DrawImageUnscaled(bmTitle, titleLeftOffset, titleTopOffset);
            for (var i = 0; i < text.Length; i++)
              g.DrawImage(bmGlyphs, leftTextOffset + i * glyphWidth, topTextOffset, new Rectangle(glyphWidth * glyphsText.IndexOf(text[i]), 0, glyphWidth, glyphHeight), GraphicsUnit.Pixel);
          }
          using (var fs = new FileStream(filePath, FileMode.Create))
            bmBanner.Save(fs, ImageFormat.Png);
        }
      }
    }
  }
}
