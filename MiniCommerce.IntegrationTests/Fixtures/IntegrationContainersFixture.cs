using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace MiniCommerce.IntegrationTests.Fixtures;

public sealed class IntegrationContainersFixture : IAsyncLifetime
{
    public const string RabbitUsername = "minicommerce";
    public const string RabbitPassword = "minicommerce-test";

    public MsSqlContainer SqlServer { get; } =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04").Build();

    public RabbitMqContainer RabbitMq { get; } =
        new RabbitMqBuilder("masstransit/rabbitmq:latest")
            .WithUsername(RabbitUsername)
            .WithPassword(RabbitPassword)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(5672)
                    .UntilCommandIsCompleted("rabbitmq-diagnostics", "-q", "ping")
                    .UntilCommandIsCompleted("rabbitmq-diagnostics", "-q", "check_running")
                    .UntilCommandIsCompleted(
                        "rabbitmq-plugins",
                        "is_enabled",
                        "rabbitmq_delayed_message_exchange"))
            .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            SqlServer.StartAsync(),
            RabbitMq.StartAsync());
    }

    public async Task CreateVirtualHostAsync(
        string virtualHost,
        CancellationToken cancellationToken = default)
    {
        await AssertCommandSucceeded(
            RabbitMq.ExecAsync(
                new[] { "rabbitmqctl", "add_vhost", virtualHost },
                cancellationToken));

        await AssertCommandSucceeded(
            RabbitMq.ExecAsync(
                new[]
                {
                    "rabbitmqctl", "set_permissions", "-p", virtualHost,
                    RabbitUsername, ".*", ".*", ".*"
                },
                cancellationToken));
    }

    public async Task WaitUntilRabbitMqIsReadyAsync(
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        ExecResult lastResult = default;

        while (DateTime.UtcNow < deadline)
        {
            lastResult = await RabbitMq.ExecAsync(
                new[] { "rabbitmq-diagnostics", "-q", "check_running" },
                cancellationToken);

            if (lastResult.ExitCode == 0)
                return;

            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        }

        throw new TimeoutException(
            $"RabbitMQ did not become ready after it was resumed. " +
            $"stdout: {lastResult.Stdout}; stderr: {lastResult.Stderr}");
    }

    public async Task DisposeAsync()
    {
        await RabbitMq.DisposeAsync();
        await SqlServer.DisposeAsync();
    }

    private static async Task AssertCommandSucceeded(Task<ExecResult> command)
    {
        var result = await command;

        Assert.True(
            result.ExitCode == 0,
            $"Container command failed. stdout: {result.Stdout}; stderr: {result.Stderr}");
    }
}

[CollectionDefinition(Name)]
public sealed class IntegrationContainersCollection
    : ICollectionFixture<IntegrationContainersFixture>
{
    public const string Name = "MiniCommerce integration containers";
}
