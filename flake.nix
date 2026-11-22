{
  description = "vscode dotnet unity environment";
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs {
        inherit system;
        config.allowUnfree = true;
      };
    in {
      devShells.${system}.default = pkgs.mkShell {
        buildInputs = with pkgs; [
          vscode.fhs
          dotnet-sdk_10
          unityhub
          mono
        (writeShellScriptBin "code" ''
          exec ${vscode.fhs}/bin/code "$@"
        '')
        ];
        shellHook = ''
          export DOTNET_ROOT=${pkgs.dotnet-sdk_10}
          export LD_LIBRARY_PATH="/usr/lib64:${pkgs.ncurses}/lib''${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
        '';
      };
    };
}
