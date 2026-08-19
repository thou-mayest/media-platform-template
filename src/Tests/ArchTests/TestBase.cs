using Profiles.Application.Abstractions;
using Profiles.Domain;
using SharedKernal.Entities;
using System.Reflection;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Presentation;

namespace CleanModular.ArchTests;

/// <summary>
/// Provides assembly anchors for all architecture tests.
///
/// Each anchor is a concrete type from the target assembly.
/// If a project is renamed or removed, the compiler catches it here
/// instead of failing silently at test runtime.
///
/// HOW TO EXTEND:
/// When you add a new module (e.g. Posts), add its anchor types here:
///
///   protected static readonly Assembly PostsDomainAssembly      = typeof(Posts.Domain.Post).Assembly;
///   protected static readonly Assembly PostsApplicationAssembly = typeof(Posts.Application.Abstractions.IPostRepository).Assembly;
///   ...
///   protected static readonly IEnumerable<Assembly> PostsModuleAssemblies = [ PostsDomainAssembly, ... ];
/// </summary>
public abstract class TestBase
{
    // ── Building Blocks ──────────────────────────────────────────
    protected static readonly Assembly SharedKernalAssembly = typeof(BaseEntity).Assembly;

    // ── Users Module ─────────────────────────────────────────────
    protected static readonly Assembly UsersDomainAssembly       = typeof(User).Assembly;
    protected static readonly Assembly UsersApplicationAssembly  = typeof(IUserRepository).Assembly;
    protected static readonly Assembly UsersInfraAssembly        = typeof(Users.Infrastracture.DependencyInjection).Assembly;
    protected static readonly Assembly UsersPresentationAssembly = typeof(Extension).Assembly;

    // All Users assemblies together — used in module isolation tests
    protected static readonly IEnumerable<Assembly> UsersModuleAssemblies =
    [
        UsersDomainAssembly,
        UsersApplicationAssembly,
        UsersInfraAssembly,
        UsersPresentationAssembly
    ];


    // ── Profiles Module ──────────────────────────────────────────
    protected static readonly Assembly ProfilesDomainAssembly = typeof(ActorProfile).Assembly;
    protected static readonly Assembly ProfilesContractsAssembly = typeof(Profiles.Contracts.IntegrationEvents.ActorProfilePublishedIntegrationEvent).Assembly;
    protected static readonly Assembly ProfilesApplicationAssembly = typeof(IActorProfileRepository).Assembly;
    protected static readonly Assembly ProfilesInfraAssembly = typeof(Profiles.Infrastructure.DependencyInjection).Assembly;
    protected static readonly Assembly ProfilesPresentationAssembly = typeof(Profiles.Presentation.Extension).Assembly;

    protected static readonly IEnumerable<Assembly> ProfilesModuleAssemblies =
    [
        ProfilesDomainAssembly,
        ProfilesContractsAssembly,
        ProfilesApplicationAssembly,
        ProfilesInfraAssembly,
        ProfilesPresentationAssembly
    ];
}
