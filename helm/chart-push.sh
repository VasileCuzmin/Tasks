#!/usr/bin/env bash
# ------------------------------------------------------------------
# [Victor Cotoman] chart-push.sh
# Description: A shell script that will push charts to Azure
# ------------------------------------------------------------------

VERSION=0.1.0
USAGE="Usage: command -ihv args"
BLOB="helmrepo0"
RG="helm-repo-rg"
CONTAINER_NAME="helm"
HELM_REPO="totalsoft"

# --- Options processing -------------------------------------------

set -Eeuo pipefail
trap cleanup SIGINT SIGTERM ERR EXIT

script_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")" &>/dev/null && pwd -P)

usage() {
  cat <<EOF
Usage: $(basename "${BASH_SOURCE[0]}") [-h] [-v] [-f] -p chart_name

Script description here.

Available options:

-h, --help      Print help message and exit
-v, --verbose   Print script debug info
-f, --flag      Flag description
-p, --param     Helm package that will be pushed
EOF
  exit
}

cleanup() {
  trap - SIGINT SIGTERM ERR EXIT
  rm index.yaml
}

setup_colors() {
  if [[ -t 2 ]] && [[ -z "${NO_COLOR-}" ]] && [[ "${TERM-}" != "dumb" ]]; then
    NOFORMAT='\033[0m' RED='\033[0;31m' GREEN='\033[0;32m' ORANGE='\033[0;33m' BLUE='\033[0;34m' PURPLE='\033[0;35m' CYAN='\033[0;36m' YELLOW='\033[1;33m'
  else
    NOFORMAT='' RED='' GREEN='' ORANGE='' BLUE='' PURPLE='' CYAN='' YELLOW=''
  fi
}

msg() {
  echo >&2 -e "${1-}"
}

die() {
  local msg=$1
  local code=${2-1} # default exit status 1
  msg "$msg"
  exit "$code"
}

parse_params() {
  # default values of variables set from params
  flag=0
  param=''

  while :; do
    case "${1-}" in
    -h | --help) usage ;;
    -v | --verbose) set -x ;;
    --no-color) NO_COLOR=1 ;;
    -f | --flag) flag=1 ;; # example flag
    -p | --param) # example named parameter
      param="${2-}"
      shift
      ;;
    -?*) die "Unknown option: $1" ;;
    *) break ;;
    esac
    shift
  done

  # check required params and arguments
  [[ -z "${param-}" ]] && die "Missing required parameter: param"

  return 0
}

parse_params "$@"
setup_colors

# helm package ${param}

msg "${GREEN}INFO: Setting environment variables"
export AZURE_STORAGE_ACCOUNT=${BLOB}
export AZURE_STORAGE_KEY=$(az storage account keys list --resource-group ${RG} --account-name ${BLOB} | grep -m 1 value | awk -F'"' '{print $4}')

# get remote index.yaml
az storage blob download-batch --destination . --pattern index.yaml --account-name helmrepo0 -s helm
msg "${GREEN}INFO: Downloaded successfully index.yaml"

helm repo index --url https://${BLOB}.blob.core.windows.net/helm/ --merge index.yaml .

az storage blob upload --container-name ${CONTAINER_NAME} --file index.yaml --name index.yaml || (msg "${RED}ERROR: Please check index.yaml" && exit 1)
az storage blob upload --container-name ${CONTAINER_NAME} --file ${param}-*.tgz --name ${param}-*.tgz

msg "${GREEN}INFO: Chart ${param} was pushed successfully to Azure."
cleanup