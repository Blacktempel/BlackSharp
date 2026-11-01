# Contributing

Everyone is welcome and encouraged to contribute. Bug reports, documentation,
tests, fixes and additional reusable components are all valuable. Keep changes
focused and follow the existing architecture, naming, documentation, formatting
and code style. Include or update tests and documentation where applicable,
and describe the affected packages, platforms and validation in the pull
request.

## Before you start

- Check existing issues and pull requests to avoid duplicating work.
- Discuss substantial features, public API changes and changes in behavior with the maintainers before implementing them.
- Report security vulnerabilities privately through the repository's security contact or GitHub's private vulnerability reporting, if available.
  Do not include exploit details in a public issue.

## Bug fixes

Describe how to reproduce the problem and what behavior you expect.
Include relevant versions, operating systems and logs, but remove credentials,
serial numbers and other sensitive data.

Add a regression test when the behavior can be tested reliably.
Unit tests should be deterministic and should not depend on a contributor's hardware,
software, network connection, local configuration, [...]. If a fix requires platform- or
device-specific validation, explain what was tested and what remains unverified.
If a test is impractical, explain why in the pull request.

## Features

Open a discussion before starting a **substantial** feature.
Explain the use case, the proposed behavior and any effect on existing
users or public APIs. Include tests and update documentation where applicable.
User-visible text should be localizable in projects that support translations.

## Pull requests

A good pull request tells reviewers:

- What problem it solves and how the change addresses it.
- Which issue it relates to, if any; use `Fixes #123` when the pull request resolves that issue.
- Which builds and tests were run, on which relevant platforms and what could not be verified.
- Whether it changes public APIs, persisted data, behavior or compatibility.
- Whether AI tools contributed to the change, as described below.

Keep unrelated refactoring, dependency updates and formatting changes out of the pull request.
Follow the existing architecture, naming, documentation, regions and formatting conventions.
Preserve a file's established encoding and line endings.
Document new APIs and non-obvious behavior.

Respect the license of every file you modify.
Do not introduce third-party code or assets unless their license permits
the intended use and any required attribution is included.

## AI-assisted contributions

Disclose AI involvement in the pull request description:

- Mark changes created entirely by AI as **AI-generated**.
- Mark changes to which an AI tool contributed as **AI-assisted**, even if you later reviewed or edited them.
- Identify the affected parts of the change.

The contributor **must understand and review the complete submission,** including generated code,
tests and documentation. The **contributor remains responsible** for correctness, security,
testing and license compliance. Keep code comments and pull request descriptions
concise and useful to human reviewers.
