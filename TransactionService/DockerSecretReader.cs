using Microsoft.Extensions.FileProviders;

public static class DockerSecretReader {
    public static string GetSecretOrEnvVar(string key, IConfiguration configuration)
    {
        const string DOCKER_SECRET_PATH = "/run/secrets/";
        if (Directory.Exists(DOCKER_SECRET_PATH))
        {
            IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
            IFileInfo fileInfo = provider.GetFileInfo(key);
            if (fileInfo.Exists)
            {
                using (var stream = fileInfo.CreateReadStream())
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    
        return configuration.GetValue<string>(key);
    }
}