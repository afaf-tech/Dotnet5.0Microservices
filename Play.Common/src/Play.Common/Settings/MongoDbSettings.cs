namespace Play.Common.Service.Settings {
    public class MongoDbSettings {
        public string Host { get; init; } // init = prevent modification
        public int Port { get; init; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}