﻿// Copyright (c) Microsoft. All rights reserved.
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Agents;

/// <summary>
/// A <see cref="KernelAgent"/> specialization bound to a <see cref="ChatHistoryChannel"/>.
/// </summary>
public abstract class ChatHistoryKernelAgent : KernelAgent, IChatHistoryHandler
{
    /// <inheritdoc/>
    protected internal sealed override IEnumerable<string> GetChannelKeys()
    {
        yield return typeof(ChatHistoryChannel).FullName!;
    }
    /// <inheritdoc/>
    protected internal sealed override Task<AgentChannel> CreateChannelAsync(ILogger logger, CancellationToken cancellationToken)
    {
        return Task.FromResult<AgentChannel>(new ChatHistoryChannel());
    }

    /// <inheritdoc/>
    protected internal sealed override Task<AgentChannel> RestoreChannelAsync(string state, ILogger logger, CancellationToken cancellationToken)
    {
        ChatHistory history =
            JsonSerializer.Deserialize<ChatHistory>(state, AgentChatSerializer.DefaultOptions) ??
            throw new KernelException("%%%"); // %%%
        return Task.FromResult<AgentChannel>(new ChatHistoryChannel(history));
    }

    /// <inheritdoc/>
    public abstract IAsyncEnumerable<ChatMessageContent> InvokeAsync(
        IReadOnlyList<ChatMessageContent> history,
        ILogger logger,
        CancellationToken cancellationToken = default);
}
