using System.Drawing.Imaging;

public class ImgCapture
{
    private int _refX = 0;
    private int _refY = 0;
    private int _imgW = 0;
    private int _imgH = 0;

    public static string filePath = "C:\\Users\\user\\Desktop\\카드이미\\";

    public ImgCapture(int refX = 0, int refY = 0, int imgW = 0, int imgH = 0)
    {
        _refX = refX;
        _refY = refY;
        _imgW = imgW;
        _imgH = imgH;
    }

    public void SetPath(string path)
    {
        filePath += path;
    }

    public void DoCaptureImage()
    {
        if (filePath != null)
        {
            if (_imgW == 0 || _imgH == 0)
                return;

            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)_imgW, (int)_imgH))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(_refX, _refY, 0, 0, bitmap.Size);
                }

                bitmap.Save(filePath, ImageFormat.Png);
            }
        }
    }

    public Bitmap GetBit()
    {
        Bitmap bitmap = new Bitmap((int)_imgW, (int)_imgH);

        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(_refX, _refY, 0, 0, bitmap.Size);
        }

        return bitmap;


    }
}
//출처: https://ddka.tistory.com/entry/C에서-화면의-특정-부분-capture하기 [Do.Log:티스토리]