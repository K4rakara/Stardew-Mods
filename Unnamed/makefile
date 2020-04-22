.PHONY: clean dirs cp dll copy zip uninstall cleanup all

DIR=$${PWD\#\#*/}
MOD_DIR="/run/media/disk/games/steam/steamapps/common/Stardew Valley/Mods/"

clean:
	@echo "(`echo ${DIR}`): Cleaning..."; \
	rm -rf ./bin; \

dirs:
	@echo "(`echo ${DIR}`): Creating directories..."; \
	mkdir -p ./bin/tmp/unpacked; \

cp:
	@cd "./src/[CP] Unnamed/"; \
	make all; \
	cd ../../; \

dll:
	@cd "./src/Unnamed/"; \
	make all; \
	cd ../../; \

copy:
	@echo "(`echo ${DIR}`): Copying parts..."; \
	cp -r "./src/[CP] Unnamed/bin/[CP] Unnamed/" ./bin/tmp/unpacked/; \
	cp -r "./src/Unnamed/bin/Unnamed/" ./bin/tmp/unpacked/; \

zip:
	@echo "(`echo ${DIR}`): Zipping parts..."; \
	zip -r -q ./bin/tmp/packed.zip ./bin/tmp/unpacked/*;\
	cp ./bin/tmp/packed.zip ./bin/; \
	rename packed `node ./tools/getv.js` ./bin/packed.zip; \

uninstall:
	@echo "(`echo ${DIR}`): Uninstalling old version..."; \
	rm -rf ${MOD_DIR}/Unnamed; \

install:
	@echo "(`echo ${DIR}`): Installing new version..."; \
	mkdir -p ${MOD_DIR}/Unnamed/; \
	cp -r ./bin/tmp/unpacked/* ${MOD_DIR}/Unnamed/; \

cleanup:
	@echo "(`echo ${DIR}`): Cleaning up..."; \
	rm -rf ./bin/tmp/; \

all:
	@echo "(`echo ${DIR}`): Making unnamed..."; \
	make clean; \
	make dirs; \
	make cp; \
	make dll; \
	make copy; \
	make zip; \
	make uninstall; \
	make install; \
	make cleanup; \