#!/bin/sh
cd "$(dirname "$(realpath -- "$0")")"

echo "Enable accessing Meta XR Simulator and Sythetic Environment Server executables"
xattr -d com.apple.quarantine ./synth_env_server/synth_env_server.app 2>/dev/null
xattr -d com.apple.quarantine SIMULATOR.so 2>/dev/null
chmod +x ./synth_env_server/*.sh

echo "Set active OpenXR runtime to: $(pwd)/meta_openxr_simulator.json"
mkdir -p /usr/local/share/openxr/1
rm /usr/local/share/openxr/1/active_runtime.json
ln -s $(pwd)/meta_openxr_simulator.json /usr/local/share/openxr/1/active_runtime.json

echo "Post installation setup completed"
