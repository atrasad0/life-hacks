//Yuri Gonçalves
using UIKit;
using System;
using System.IO;
using Foundation;

public async Task<string> RedimensionaImg(string fullPathImage, int tamanhoLimiteImagem)
{
    var qualidade = 100;

    try
    {
        var tamanho = new FileInfo(fullPathImage).Length;
        var baseFileUri = fullPathImage.Replace(".jpg", "", StringComparison.OrdinalIgnoreCase);

        while (tamanho > tamanhoLimiteImagem)
        {
            UIImage img;
            using (var stream = File.OpenRead(fullPathImage))
            using (var data = NSData.FromStream(stream))
                img = UIImage.LoadFromData(data);

            if (File.Exists(fullPathImage))
                File.Delete(fullPathImage);

            fullPathImage = $@"{baseFileUri}_{qualidade}.jpg";

            var compressImg = img.AsJPEG((nfloat)qualidade / 100).ToArray();

            using (var newFile = new FileStream(fullPathImage, FileMode.Create))
            {
                newFile.Write(compressImg, 0, compressImg.Length);
                newFile.Flush();
                newFile.Close();
            }

            qualidade -= 25;
            tamanho = new FileInfo(fullPathImage).Length;

            if (qualidade < 0)
                throw new Exception("A compressão está no limite e sua imagem continua maior que o tamanho permitido, tente reduzir o tamanho da foto.");
        }

        return fullPathImage;
    }
    catch (Exception e)
    {
        logger.Error(e, $@"Erro metodo de redimencionar imagem, ultima tentativa de compressao: {qualidade}");
        await applicationController.AlertAsync("ERRO",e.Message, "OK");

        return String.Empty;
    }
}
