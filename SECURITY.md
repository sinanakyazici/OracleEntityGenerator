# Security Policy

## Reporting a Vulnerability

Please do not open a public issue for security-sensitive reports.

If you discover a vulnerability, contact the maintainer privately through GitHub profile contact options. Include:

- A clear description of the issue.
- Steps to reproduce.
- Impact and affected versions, if known.
- Any relevant logs with secrets removed.

## Credential Handling Expectations

Oracle Entity Generator must not:

- Hardcode database credentials.
- Store passwords in plain text.
- Write secrets to generated files.
- Log passwords or full connection strings containing passwords.

If connection profiles are expanded in the future, passwords must be stored using secure platform storage or omitted entirely.
