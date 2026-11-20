// Kurallar: docs/standartlar/git-akisi.md
module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [2, 'always', ['feat', 'fix', 'contract', 'refactor', 'test', 'docs', 'chore', 'ci', 'perf', 'db']],
    'scope-empty': [2, 'never'],
    'scope-enum': [2, 'always', [
      'api', 'api/identity', 'api/tenancy', 'api/catalog', 'api/customers', 'api/scheduling', 'api/sales',
      'api/reviews', 'api/notifications', 'api/integrations', 'api/platform',
      'salon-panel', 'platform-panel', 'rezervasyon', 'tanitim', 'isletme', 'musteri',
      'ui-web', 'omvila-ui', 'tokens', 'contracts', 'docs', 'repo',
    ]],
    'subject-case': [0],
    'header-max-length': [2, 'always', 100],
    'subject-full-stop': [2, 'never', '.'],
  },
};
