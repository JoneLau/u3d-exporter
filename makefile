# /////////////////////////////////////////////////////////////////////////////
#  settings
# /////////////////////////////////////////////////////////////////////////////

# settings (mac)
unity_path = /Applications/Unity/Unity.app/Contents/

# unit essential (mac)
compiler = $(unity_path)/Mono/bin/gmcs

unity_engine_dll = $(unity_path)/Managed/UnityEngine.dll
unity_engine_ui_dll = $(unity_path)/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll
unity_editor_dll = $(unity_path)/Managed/UnityEditor.dll
json_dll = ./Assets/vendors/JsonNet-Lite/Newtonsoft.Json.dll

# Utilities.
MKDIR = mkdir -p
RM = rm -f
RMDIR = rm -rf

# Target
dest = ./bin
target = $(dest)/Editor/u3d-exporter.dll

# /////////////////////////////////////////////////////////////////////////////
# builds
# /////////////////////////////////////////////////////////////////////////////

.PHONY: clean rebuild

all: $(target)
clean:
	$(RMDIR) $(dest)
rebuild: |clean all

# defines
editor_define = -d:UNITY_EDITOR

# /////////////////////////////////////////////////////////////////////////////
# targets
# /////////////////////////////////////////////////////////////////////////////

# get sources
src_dirs += ./Assets/Scripts/
src_dirs += ./Assets/Editor/
src = $(wildcard $(addsuffix *.cs,$(src_dirs)))

# resources
# resources += -resource:Assets/Resource/pixel.png

# argument
args = $(editor_define) $(resources) -r:$(unity_engine_dll),$(unity_engine_ui_dll),$(unity_editor_dll),$(json_dll)

# do the build
$(target):
	@echo "========================================================"
	@echo compiling u3d-exporter.dll...
	@echo "========================================================"
	@echo
	$(RMDIR) $(dest)
	$(MKDIR) $(dest)/Editor
	cp -r ./Assets/shaders/ $(dest)/shaders/
	cp -r ./Assets/vendors/ $(dest)/vendors/
	$(compiler) -target:library -out:$(target) $(args) $(src)
	@echo done!
	@echo
