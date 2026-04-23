#compdef mdev

_mdev() {
    local -a branches
    branches=("${(@f)$(git branch --format='%(refname:short)' 2>/dev/null)}")
    
    _arguments \
        '(-h --help)'{-h,--help}'[Show help message]' \
        '(-r --refresh)'{-r,--refresh}'[Stash changes, pull latest and pop stash]' \
        '--install-completions[Install zsh autocomplete script]' \
        '1: :->branches'

    case "$state" in
        branches)
            _describe 'branches' branches
            ;;
    esac
}
