using Orleans.Contrib.UniversalSilo;

/// <summary>
/// Override methods in this class to take over how the silo is configured
/// </summary>
class SiloConfigurator : Configuration.SiloConfigurator
{
  public override Configuration.SiloConfiguration SiloConfiguration =>
    base.SiloConfiguration
      .With(_c => _c.ServiceId = "SchemaLand.Api");

  //public override ClusteringConfiguration ClusteringConfiguration =>
  //    base.ClusteringConfiguration;

  //public override StorageProviderConfiguration StorageProviderConfiguration =>
  //    base.StorageProviderConfiguration;

  //public override TelemetryConfiguration TelemetryConfiguration =>
  //    base.TelemetryConfiguration;

  //public override Orleans.Hosting.ISiloBuilder ConfigureServices(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureServices(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureClustering(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureClustering(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureStorageProvider(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureStorageProvider(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureReminderService(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureReminderService(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureApplicationInsights(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureApplicationInsights(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureDashboard(IConfiguration configuration, UniversalSiloConfiguration siloSettings, Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureDashboard(configuration, siloSettings, siloBuilder);

  //public override Orleans.Hosting.ISiloBuilder ConfigureApplicationParts(Orleans.Hosting.ISiloBuilder siloBuilder) =>
  //    base.ConfigureApplicationParts(siloBuilder);

  public SiloConfigurator() : base()
  { }
}
